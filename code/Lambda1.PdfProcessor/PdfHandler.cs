using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS; 
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
        context.Logger.LogInformation("test");
    }
}