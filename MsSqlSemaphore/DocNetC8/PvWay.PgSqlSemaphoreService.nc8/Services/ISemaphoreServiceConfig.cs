namespace PvWay.PgSqlSemaphoreService.nc8.Services;

internal interface ISemaphoreServiceConfig
{
    string SchemaName { get; }
    string TableName { get; }
    Func<Task<string>> GetCsAsync { get; }
    Action<Exception> LogException { get; }
    Action<string>? LogInfo { get; } 
}