using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

internal class SerilogConsoleFactory: ILoggerServiceFactory<ISeriConsoleLoggerService>
{
    private readonly ILoggerServiceConfig _config;

    public SerilogConsoleFactory(
        ILoggerServiceConfig config)
    {
        _config = config;
    }
    
    public ISeriConsoleLoggerService CreateLoggerService()
    {
        return new SerilogConsoleService(
            _config, new SerilogConsoleWriter());
    }
}