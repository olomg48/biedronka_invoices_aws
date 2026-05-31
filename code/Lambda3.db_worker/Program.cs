using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;

namespace MockProcessor;

public class Program
{
    private static async Task Main(string[] args)
    {
        await LambdaBootstrapBuilder.CreateServerlessCodeFirstInstanceAsync()
            .RunAsync();
    }
}

public class Function
{
    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogInformation($"Start: {record.MessageId}");
        }
    }
}