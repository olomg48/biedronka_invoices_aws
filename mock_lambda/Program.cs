using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

var lambdaFunction = new HelloWorldLambda.Function();

// Jawnie podajemy <string, string> i opakowujemy synchroniczną metodę w Task
await LambdaBootstrapBuilder.Create<string, string>(
    (input, context) => Task.FromResult(lambdaFunction.FunctionHandler(input, context)), 
    new DefaultLambdaJsonSerializer()
)
.Build()
.RunAsync();