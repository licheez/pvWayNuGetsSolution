using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Mute.nc8;

internal sealed class MuteLoggerService(
    ILoggerServiceConfig config) :
    LoggerService.nc8.LoggerService(
        new MuteLogWriter(), config),
    IMuteLoggerService;
    
internal sealed class MuteLoggerService<T>(
    ILoggerServiceConfig config) :
    LoggerService<T>(new MuteLogWriter(), config),
    IMuteLoggerService<T>;