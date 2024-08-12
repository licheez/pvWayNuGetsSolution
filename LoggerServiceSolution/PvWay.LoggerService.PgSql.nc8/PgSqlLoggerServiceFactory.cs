using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.PgSql.nc8;

public class PgSqlLoggerServiceFactory(
    Func<SqlRoleEnu, Task<string>> getCsAsync,
    IConfiguration config,
    SeverityEnu minLogLevel = SeverityEnu.Trace)
    : ILoggerServiceFactory<IPgSqlLoggerService>
{
    public IPgSqlLoggerService CreateLoggerService()
    {
        var lsConfig = new LoggerServiceConfig(minLogLevel);
        var lwConfig = new PgSqlLogWriterConfig(config);
        var csp = new PgSqlConnectionStringProvider(getCsAsync);
        var logWriter = new PgSqlLogWriter(csp, lwConfig);
        return new PgSqlLoggerService(lsConfig, logWriter);
    }
}