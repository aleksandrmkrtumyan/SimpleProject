using System.Text.Json;
using Grpc.Core;

namespace Backoffice.Client.ServiceProxies;

public class Executor
{
    private readonly Backoffice.Executor.ExecutorClient? executorClient;

    internal Executor(Backoffice.Executor.ExecutorClient? executorClient)
    {
        this.executorClient = executorClient;
    }

    internal async Task<TResult?> Execute<TResult>(ExecuteInput input)
    {
        var headers = new Metadata();
        
        var executionResult = await executorClient.ExecuteAsync(input, headers);
        
        return JsonSerializer.Deserialize<TResult>(executionResult.Result);
    }

    internal async Task Execute(ExecuteInput input)
    {
        var headers = new Metadata();
        
        await executorClient.ExecuteAsync(input, headers);
    }
}