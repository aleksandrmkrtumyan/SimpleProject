using System.Text.Json;
using Grpc.Core;

namespace Portal.Client.ServiceProxies;


public class Executor
{
    private readonly Portal.Executor.ExecutorClient executorClient;
    // private readonly UserSession userSession;

    internal Executor(Portal.Executor.ExecutorClient executorClient
        // , UserSession userSession
        )
    {
        this.executorClient = executorClient;
        // this.userSession = userSession;
    }

    internal async Task<TResult> Execute<TResult>(ExecuteInput input)
    {
        var headers = new Metadata();

        // проверяем чтобы сессия была загружена
        // await userSession.EnsureLoaded();
            
        // if (userSession.Token != null)
        //     headers.Add("Authorization", $"Bearer {userSession.Token}");
        // else
        // {
        //     if (userSession.AnonymousUserId!=null)
        //         headers.Add("AnonymousUserId", userSession.AnonymousUserId.ToString());
        // }


        var executionResult = await executorClient.ExecuteAsync(input, headers);

        return JsonSerializer.Deserialize<TResult>(executionResult.Result);
    }
        
    internal async Task Execute(ExecuteInput input)
    {
        var headers = new Metadata();

        // проверяем чтобы сессия была загружена
        // await userSession.EnsureLoaded();

        // if (userSession.Token != null)
        //     headers.Add("Authorization", $"Bearer {userSession.Token}");
        // else if (userSession.AnonymousUserId != null)
        //     headers.Add("AnonymousUserId", userSession.AnonymousUserId.ToString());

        await executorClient.ExecuteAsync(input, headers);
    }
}