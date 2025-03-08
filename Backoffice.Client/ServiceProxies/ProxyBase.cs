using System.Text.Json;

namespace Backoffice.Client.ServiceProxies;

public abstract class ProxyBase<TInput, TResult>
{
    #region Fields
    
    private readonly Executor executor;

    #endregion Fields
    
    #region Constructor

    protected ProxyBase(Executor executor)
    {
        this.executor = executor;
    }
    
    #endregion Constructor
    
    public Task<TResult?> Execute(TInput input)
    {
        ExecuteInput executeInput = new ExecuteInput
        {
            CommandOrQueryName = GetType().Name.Replace("Proxy", string.Empty),
            Input = JsonSerializer.Serialize(input)
        };
        return executor.Execute<TResult>(executeInput);
    }
}

public abstract class ProxyBaseNoResult<TInput>
{
    #region Fields

    private readonly Executor executor;

    #endregion Fields

    #region Constructor

    protected ProxyBaseNoResult(Executor executor)
    {
        this.executor = executor;
    }

    #endregion Constructor

   
    public Task Execute(TInput input)
    {
        ExecuteInput executeInput = new ExecuteInput
        {
            CommandOrQueryName = GetType().Name.Replace("Proxy", string.Empty),
            Input = JsonSerializer.Serialize(input)
        };
        return executor.Execute(executeInput);
    }
}

public abstract class ProxyBaseNoInputNoResult
{
    #region Fields

    private readonly Executor executor;

    #endregion Fields

    #region Constructor

    protected ProxyBaseNoInputNoResult(Executor executor)
    {
        this.executor = executor;
    }

    #endregion Constructor

    
    public Task Execute()
    {
        ExecuteInput executeInput = new ExecuteInput
        {
            CommandOrQueryName = GetType().Name.Replace("Proxy", string.Empty)
        };
        return executor.Execute(executeInput);
    }
}