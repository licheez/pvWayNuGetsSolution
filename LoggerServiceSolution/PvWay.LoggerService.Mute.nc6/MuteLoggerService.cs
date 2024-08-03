using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Mute.nc6;

internal sealed class MuteLoggerService :
    LoggerService.nc6.LoggerService,
    IMuteLoggerService
{
    public MuteLoggerService(ILoggerServiceConfig config) : base(config, new MuteLogWriter())
    {
    }
}

internal sealed class MuteLoggerService<T> :
    LoggerService<T>,
    IMuteLoggerService<T>
{
    public MuteLoggerService(ILoggerServiceConfig config) : base(config, new MuteLogWriter())
    {
    }
}