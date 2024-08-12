using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public class PgSqlLoggerServiceFactory: ILoggerServiceFactory<IPgSqlLoggerService>
{
    private readonly Func<SqlRoleEnu, Task<string>> _getCsAsync;
    private readonly IConfiguration _config;
    private readonly SeverityEnu _minLogLevel;

    public PgSqlLoggerServiceFactory(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration config,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        _getCsAsync = getCsAsync;
        _config = config;
        _minLogLevel = minLogLevel;
    }
    
    public IPgSqlLoggerService CreateLoggerService()
    {
        var lsConfig = new LoggerServiceConfig(_minLogLevel);
        var lwConfig = new PgSqlLogWriterConfig(_config);
        var csp = new PgSqlConnectionStringProvider(_getCsAsync);
        var logWriter = new PgSqlLogWriter(csp, lwConfig);
        return new PgSqlLoggerService(lsConfig, logWriter);
    }
}