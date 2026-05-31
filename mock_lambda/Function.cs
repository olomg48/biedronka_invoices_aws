using Amazon.Lambda.Core;

// Ten atrybut jest kluczowy – mówi Lambdzie, jak ma zamieniać JSON-y na obiekty C#
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorldLambda;

public class Function
{
    public string FunctionHandler(string input, ILambdaContext context)
    {
        // Logi automatycznie trafią do AWS CloudWatch
        context.Logger.LogInformation("Lambda została uruchomiona.");
        string bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
        return $"Hello World! Twoja wiadomość to: {bucketName}";
    }
}