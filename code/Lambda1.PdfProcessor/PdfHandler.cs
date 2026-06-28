using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.S3Events;
using Amazon.SQS; 
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
namespace PdfProcessor.Functions;

public class PdfHandler
{   
    const string endpointUrl = "https://api.mistral.ai/v1/ocr";
    string bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
    string mistralApiKey = Environment.GetEnvironmentVariable("MISTRAL_API_KEY");
    private readonly IAmazonSQS _sqsClient;
    public PdfHandler(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;
    }
    public class MistralOcrResponse
    {
        public List<MistralOcrPage> Pages { get; set; }
    }

    public class MistralOcrPage
    {
        public int Index { get; set; }
        public string Markdown { get; set; }
    }
    public async Task ProcessPdf(SQSEvent sqsEvent, ILambdaContext context)
    {   
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
        var s3Config = new AmazonS3Config
        {
            ServiceURL = "http://localstack_main_compose:4566",
            ForcePathStyle = true
        };

        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogInformation($"Przetwarzanie wiadomości SQS ID: {record.MessageId}");
            var s3Event = JsonSerializer.Deserialize<S3Event>(record.Body, options);
            if (s3Event?.Records != null)
            {
                foreach (var s3Record in s3Event.Records)
                {   
                    
                    string rawObjectKey = s3Record.S3.Object.Key;
                    string objectKey = WebUtility.UrlDecode(rawObjectKey);
                    string fileName = Path.GetFileName(objectKey);
                    using var s3Client = new AmazonS3Client("localstack", "localstack", s3Config);
                    using var s3Response = await s3Client.GetObjectAsync(bucketName, objectKey);
                    using var ms = new MemoryStream();
                    await s3Response.ResponseStream.CopyToAsync(ms);
                    string base64Document = Convert.ToBase64String(ms.ToArray());
                    Console.WriteLine($"objectKey: {objectKey}");
                    Console.WriteLine($"fileName: {fileName}");
                    var requestBody = new
                    {
                        model = "mistral-ocr-latest",
                        document = new
                        {
                            type = "document_url",
                            document_url = $"data:application/pdf;base64,{base64Document}"
                        },
                        include_image_base64 = false
                    };

                    string jsonPayload = JsonSerializer.Serialize(requestBody);
                    Console.WriteLine("Wysyłanie zapytania do Mistral OCR...");
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mistralApiKey);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(endpointUrl, content);
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var ocrData = JsonSerializer.Deserialize<MistralOcrResponse>(jsonResult, options);

                    string fullMarkdown = string.Join("\n\n", ocrData.Pages.Select(p => p.Markdown));

                    string outputKey = objectKey
                        .Replace("raw/", "processed/")
                        .Replace(".pdf", ".md", StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine($"Zapisywanie wyniku do S3: {outputKey}...");
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName, 
                        Key = outputKey,
                        ContentBody = fullMarkdown, 
                        ContentType = "text/markdown" 
                    };

                    
                    var putResponse = await s3Client.PutObjectAsync(putRequest);
                    Console.WriteLine($"Zapisano do S3: {outputKey}");
                }
            }
        }
    }


}