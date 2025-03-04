using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Application;
using Autofac;
using Backoffice.Application.Queries.Administrators;
using Domain;
using Grpc.Core;
using Persistence;

namespace Backoffice.Server.Services;

public class ExecutorService : Executor.ExecutorBase
{
    /// <summary>
    /// Information about the command/request for their dynamic invocation.
    /// </summary>
    private class CommandOrQueryInfo
    {
        public CommandOrQueryInfo(Type commandOrQueryType)
        {
            // Using reflection, we obtain all the necessary information about commands/requests.
            CommandOrQueryType = commandOrQueryType;
            ExecuteMethod = commandOrQueryType.GetMethods()
                .First(m => m.Name == "Execute");
            InputType = ExecuteMethod.GetParameters().FirstOrDefault()?.ParameterType;
            HasReturnValue = ExecuteMethod.ReturnType.IsGenericType;
            if (HasReturnValue)
            {
                // We take the first generic argument of Task<>
                ReturnValueType = ExecuteMethod.ReturnType.GenericTypeArguments.FirstOrDefault();

                // If it does not return a model (or a set of models) and not a simple type (or a set of simple types),
                // we consider that there is no return type.
                var returnType =
                    ReturnValueType.IsGenericType &&
                    ReturnValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        ? ReturnValueType.GenericTypeArguments.First()
                        : ReturnValueType;

                if (returnType.Namespace != nameof(System) &&
                    !returnType.Name.EndsWith("Model")
                    && !returnType.Name.StartsWith("DataResult")

                    //&& !returnType.GetInterfaces().Contains(typeof(IEnumerable<>))
                   )
                {
                    HasReturnValue = false;
                    ReturnValueType = null;
                }
            }
        }

        /// <summary>
        /// Type (System.Type) Command/Query
        /// </summary>
        public Type CommandOrQueryType { get; }

        /// <summary>
        /// Information about method Execute command/query
        /// </summary>
        public MethodInfo ExecuteMethod { get; }

        /// <summary>
        /// The type (System.Type) of the input argument.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        /// The type (System.Type) of the return value.
        /// </summary>
        public Type ReturnValueType { get; }

        /// <summary>
        /// Has return value?
        /// </summary>
        public bool HasReturnValue { get; }
    }

    #region Fields

    private readonly ILifetimeScope lifetimeScope;

    // private readonly IDateTimeProvider dateTimeProvider;
    private readonly IChangesSaver changesSaver;

    // private readonly IUserInfoProvider userInfoProvider;
    private readonly ScopeProvider scopeProvider;

    private static readonly Dictionary<string, CommandOrQueryInfo> CommandsAndQueries =
        new Dictionary<string, CommandOrQueryInfo>();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes the ExecutorService by gathering information about all commands/requests in the subsystem.
    /// </summary>
    public static void Init()
    {
        var commandsAndQueriesTypes =
            Assembly
                .GetAssembly(typeof(GetAllAdministratorsQuery))
                .GetTypes()
                .Where(t => t.Name.EndsWith("Query") || t.Name.EndsWith("Command"));

        foreach (var commandOrQuery in commandsAndQueriesTypes)
        {
            CommandOrQueryInfo commandOrQueryInfo = new CommandOrQueryInfo(commandOrQuery);
            CommandsAndQueries.Add(commandOrQuery.Name, commandOrQueryInfo);
        }
    }

    public ExecutorService(
        ILifetimeScope lifetimeScope,
        IChangesSaver changesSaver,
        ScopeProvider scopeProvider
    )
    {
        this.lifetimeScope = lifetimeScope;
        this.changesSaver = changesSaver;
        this.scopeProvider = scopeProvider;
    }

    #endregion Constructors

    public override async Task<ExecuteResult> Execute(ExecuteInput request, ServerCallContext context)
    {
        var currentDate = DateTime.Now;
        // извлекаем информацию о команде или запросе
        CommandOrQueryInfo commandOrQueryInfo = CommandsAndQueries[request.CommandOrQueryName];

        // берём команду или запрос из DI-контейнера
        var command = lifetimeScope.Resolve(commandOrQueryInfo.CommandOrQueryType!);

        List<object> inputParams = new List<object>();
        if (!string.IsNullOrWhiteSpace(request.Input))
        {
            // десериализуем входящие данные
            var input = JsonSerializer.Deserialize(request.Input, commandOrQueryInfo.InputType);
            inputParams.Add(input);
        }

        // получаем Task, выполняющий команду или запрос; ждём его выполнения 
        Task executionTask = commandOrQueryInfo.ExecuteMethod
            .Invoke(command, inputParams.ToArray()) as Task;
        await executionTask;

        Process currentProc = Process.GetCurrentProcess();
        var period = (DateTime.Now - currentDate).TotalSeconds;

        var resultSize = 0;
        ExecuteResult executeResult = new ExecuteResult();
        // получаем результат выполнения команды/запроса
        if (commandOrQueryInfo.HasReturnValue)
        {
            var resultProperty = typeof(Task<>).MakeGenericType(commandOrQueryInfo.ReturnValueType)
                .GetProperty("Result");
            var result = resultProperty.GetValue(executionTask);

            var serializedResult = JsonSerializer.Serialize(result);
            resultSize = serializedResult.Length * 2 / 1024;
            // возвращаем сериализованный результат
            executeResult.Result = serializedResult;
        }

        InformationOfRequest informationOfRequest = new InformationOfRequest()
        {
            DateAndTime = currentDate,
            CommandOrRequest = "Backoffice-" + request.CommandOrQueryName,
            InputParams = request.Input,
            ResponseTime = period,
            ResponseSize = resultSize,
            ScopeId = scopeProvider.ScopeId
        };

        var informationOfRequestRepository = lifetimeScope.Resolve<IRepository<InformationOfRequest>>();

        informationOfRequestRepository.Add(informationOfRequest);

        await changesSaver.SaveChangesAsync();

        return executeResult;
    }
}