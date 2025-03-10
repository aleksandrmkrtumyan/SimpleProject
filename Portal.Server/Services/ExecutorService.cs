using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Application;
using Autofac;
using Domain;
using Grpc.Core;
using Persistence;
using Portal.Application.Queries.Administrators;

namespace Portal.Server.Services;

public class ExecutorService : Executor.ExecutorBase
{
    /// <summary>
    /// Информация о команде/запросе для их динамического вызова
    /// </summary>
    private class CommandOrQueryInfo
    {
        public CommandOrQueryInfo(Type commandOrQueryType)
        {
            // через reflection получаем всю необходимую информацию о командах/запросах
            CommandOrQueryType = commandOrQueryType;
            ExecuteMethod = commandOrQueryType.GetMethods()
                .First(m => m.Name == "Execute");
            InputType = ExecuteMethod.GetParameters().FirstOrDefault()?.ParameterType;
            HasReturnValue = ExecuteMethod.ReturnType.IsGenericType;
            if (HasReturnValue)
            {
                // Берём первый generic-аргумент у Task<>
                ReturnValueType = ExecuteMethod.ReturnType.GenericTypeArguments.FirstOrDefault();

                // если возвращается не модель (или их набор) и не простой тип (или их набор),
                // то считаем, что возвращаемого типа нет
                var returnType =
                    ReturnValueType.IsGenericType &&
                    ReturnValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        ? ReturnValueType.GenericTypeArguments.First()
                        : ReturnValueType;

                if (returnType.Namespace != nameof(System) &&
                    !returnType.Name.EndsWith("Model")
                    && !returnType.Name.StartsWith("DataResult")
                    //TODO: Проконсультироваться с Женей
                    //&& !returnType.GetInterfaces().Contains(typeof(IEnumerable<>))
                   )
                {
                    HasReturnValue = false;
                    ReturnValueType = null;
                }
            }
        }

        /// <summary>
        /// Тип (System.Type) команды/запроса
        /// </summary>
        public Type CommandOrQueryType { get; }

        /// <summary>
        /// Информация о методе Execute команды/запроса
        /// </summary>
        public MethodInfo ExecuteMethod { get; }

        /// <summary>
        /// Тип (System.Type) входного аргумента
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        /// Тип (System.Type) возвращаемого значения
        /// </summary>
        public Type ReturnValueType { get; }

        /// <summary>
        /// Есть возвращаемое значение?
        /// </summary>
        public bool HasReturnValue { get; }
    }

    #region Fields

    private readonly ILifetimeScope lifetimeScope;

    private static readonly Dictionary<string, CommandOrQueryInfo> CommandsAndQueries =
        new Dictionary<string, CommandOrQueryInfo>();

    private readonly IChangesSaver changesSaver;

    // private readonly IDateTimeProvider dateTimeProvider;
    // private readonly IUserInfoProvider userInfoProvider;
    private readonly ScopeProvider scopeProvider;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Инициализирует ExecutorService, собирая информацию обо всех командах/запросах в подсистеме
    /// </summary>
    public static void Init()
    {
        var commandsAndQueriesTypes =
            Assembly
                .GetAssembly(typeof(GetAdministratorsQuery))
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
        // IDateTimeProvider dateTimeProvider, 
        // IUserInfoProvider userInfoProvider, 
        ScopeProvider scopeProvider
    )
    {
        this.lifetimeScope = lifetimeScope;
        // this.dateTimeProvider = dateTimeProvider;
        // this.userInfoProvider = userInfoProvider;
        this.changesSaver = changesSaver;
        this.scopeProvider = scopeProvider;
    }

    #endregion Constructors

    public override async Task<ExecuteResult> Execute(ExecuteInput request
        , ServerCallContext context
    )
    {
        var currentDate = DateTime.Now; // dateTimeProvider.Now;

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
            CommandOrRequest = "Portal-" + request.CommandOrQueryName,
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