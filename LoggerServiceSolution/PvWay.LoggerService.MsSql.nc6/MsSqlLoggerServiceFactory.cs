using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

internal class MsSqlLoggerServiceFactory: ILoggerServiceFactory<IMsSqlLoggerService>
{
    private readonly Func<SqlRoleEnu, Task<string>> _getCsAsync;
    private readonly IConfiguration _config;
    private readonly SeverityEnu _minLogLevel;

    public MsSqlLoggerServiceFactory(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration config,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        _getCsAsync = getCsAsync;
        _config = config;
        _minLogLevel = minLogLevel;
    }
    
    public IMsSqlLoggerService CreateLoggerService()
    {
        var lsConfig = new LoggerServiceConfig(_minLogLevel);
        var lwConfig = new MsSqlLogWriterConfig(_config);
        var csp = new MsSqlConnectionStringProvider(_getCsAsync);
        var logWriter = new MsSqlLogWriter(csp, lwConfig);
        return new MsSqlLoggerService(lsConfig, logWriter);
    }
}