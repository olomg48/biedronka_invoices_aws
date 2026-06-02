using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS; 
using Amazon.Lambda.S3Events;
using System.Text.Json;
namespace PdfProcessor.Functions;

public class PdfHandler
{   
    private readonly IAmazonSQS _sqsClient;
    public PdfHandler(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;
    }
    
    public async Task ProcessPdf(SQSEvent sqsEvent, ILambdaContext context)
    {   
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogInformation($"Przetwarzanie wiadomości SQS ID: {record.MessageId}");
            var s3Event = JsonSerializer.Deserialize<S3Event>(record.Body, options);
            if (s3Event?.Records != null)
            {
                foreach (var s3Record in s3Event.Records)
                {
                    string bucketName = s3Record.S3.Bucket.Name;
                    string objectKey = s3Record.S3.Object.Key;

                    context.Logger.LogInformation($"Record: {objectKey}");
                }
            }
        }
    }
}