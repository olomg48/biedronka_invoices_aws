using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents; // <-- Dodane dla jawnego typu <SQSEvent>
using Amazon.SQS;
using PdfProcessor.Functions;

namespace PdfProcessor;

public class Program
{
    private static async Task Main(string[] args)
    {
        var sqsConfig = new AmazonSQSConfig();
        sqsConfig.ServiceURL = "http://localhost:4566";
        using var sqsClient = new AmazonSQSClient(sqsConfig);
        var pdfHandler = new PdfHandler(sqsClient);
        await LambdaBootstrapBuilder.Create<SQSEvent>(pdfHandler.ProcessPdf, new DefaultLambdaJsonSerializer())
            .Build()
            .RunAsync();
    }
}