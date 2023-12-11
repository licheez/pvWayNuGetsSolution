using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.UTest.nc8;

internal sealed class UTestLoggerService(
    IUTestLogWriter logWriter, 
    ILoggerServiceConfig config) : 
    LoggerService.nc8.LoggerService(logWriter, config), 
    IUTestLoggerService;