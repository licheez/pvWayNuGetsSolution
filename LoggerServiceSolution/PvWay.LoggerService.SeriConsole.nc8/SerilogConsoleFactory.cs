using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal class SerilogConsoleFactory(ILoggerServiceConfig config) : 
    ILoggerServiceFactory<ISeriConsoleLoggerService>
{
    public ISeriConsoleLoggerService CreateLoggerService()
    {
        return new SerilogConsoleService(
            config, new SerilogConsoleWriter());
    }
}