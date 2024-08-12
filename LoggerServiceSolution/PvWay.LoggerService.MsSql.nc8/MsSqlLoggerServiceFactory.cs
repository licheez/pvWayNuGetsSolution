using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

internal class MsSqlLoggerServiceFactory(
    Func<SqlRoleEnu, Task<string>> getCsAsync,
    IConfiguration config,
    SeverityEnu minLogLevel = SeverityEnu.Trace)
    : ILoggerServiceFactory<IMsSqlLoggerService>
{
    public IMsSqlLoggerService CreateLoggerService()
    {
        var lsConfig = new LoggerServiceConfig(minLogLevel);
        var lwConfig = new MsSqlLogWriterConfig(config);
        var csp = new MsSqlConnectionStringProvider(getCsAsync);
        var logWriter = new MsSqlLogWriter(csp, lwConfig);
        return new MsSqlLoggerService(lsConfig, logWriter);
    }
}