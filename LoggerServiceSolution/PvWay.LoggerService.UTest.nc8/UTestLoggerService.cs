using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.UTest.nc8;

internal sealed class UTestLoggerService(
    ILoggerServiceConfig config,
    IUTestLogWriter logWriter)
    :
        LoggerService.nc8.LoggerService(config, logWriter),
        IUTestLoggerService;

internal sealed class UTestLoggerService<T>(
    ILoggerServiceConfig config,
    IUTestLogWriter logWriter)
    :
        LoggerService<T>(config, logWriter),
        IUTestLoggerService<T>;