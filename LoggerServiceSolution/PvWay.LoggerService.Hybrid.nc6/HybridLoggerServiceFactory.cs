using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Hybrid.nc6;

internal class HybridLoggerServiceFactory : ILoggerServiceFactory<IHybridLoggerService>
{
    private readonly SeverityEnu _minLogLevel;
    private readonly ILogWriter[] _logWriters;

    public HybridLoggerServiceFactory(
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        params ILogWriter[] logWriters)
    {
        _minLogLevel = minLogLevel;
        _logWriters = logWriters;
    }
    
    public IHybridLoggerService CreateLoggerService()
    {
        var lsConfig = new LoggerServiceConfig(_minLogLevel);
        var hConfig = new HybridLoggerConfig(_logWriters);
        return new HybridLoggerService(lsConfig, hConfig);
    }
}
