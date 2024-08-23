using Microsoft.Extensions.Configuration;

namespace PvWay.PgSqlSemaphoreService.nc8.Services;

internal class SemaphoreServiceConfig(
    string schemaName,
    string tableName,
    Func<Task<string>> getCsAsync,
    Action<Exception>? logException,
    Action<string>? logInfo)
    : ISemaphoreServiceConfig
{
    public SemaphoreServiceConfig(IConfiguration config,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException = null,
        Action<string>? logInfo = null): this(
        config["schemaName"]??"public",
        config["tableName"]??"Semaphore",
        getCsAsync, 
        logException, logInfo)
    {
    }

    public string SchemaName { get; } = schemaName;
    public string TableName { get; } = tableName;
    public Func<Task<string>> GetCsAsync { get; } = getCsAsync;

    public Action<Exception> LogException { get; } = logException??Console.WriteLine;
    public Action<string>? LogInfo { get; } = logInfo;
}