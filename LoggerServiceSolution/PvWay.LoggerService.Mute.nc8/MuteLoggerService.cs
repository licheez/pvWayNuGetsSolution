using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.Mute.nc8;

internal sealed class MuteLoggerService(
    ILoggerServiceConfig config,
    IMuteLogWriter logWriter) :
    LoggerService.nc8.LoggerService(config, logWriter),
    IMuteLoggerService;

internal sealed class MuteLoggerService<T>(
    ILoggerServiceConfig config,
    IMuteLogWriter logWriter) :
    LoggerService<T>(config, logWriter),
    IMuteLoggerService<T>;