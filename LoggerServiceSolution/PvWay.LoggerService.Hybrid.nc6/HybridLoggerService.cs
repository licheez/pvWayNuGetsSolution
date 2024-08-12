using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Hybrid.nc6;

internal class HybridLoggerService: LoggerService.nc6.LoggerService, IHybridLoggerService
{
    // ReSharper disable once MemberCanBeProtected.Global
    public HybridLoggerService(
        ILoggerServiceConfig config, 
        HybridLoggerConfig hConfig) : base(config, hConfig.LogWriters)
    {
    }
}

internal class HybridLoggerService<T>: HybridLoggerService, IHybridLoggerService<T>
{
    public HybridLoggerService(
        ILoggerServiceConfig config,
        HybridLoggerConfig hConfig) : base(config, hConfig)
    {
    }
}