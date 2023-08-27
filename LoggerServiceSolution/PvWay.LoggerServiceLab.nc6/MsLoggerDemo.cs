using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal class MsLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        var logger = new LoggerFactory()
            .CreateLogger<MsLoggerDemo>();

        var msLs = PvWayLoggerService.CreateMsLoggerService(logger);

        await msLs.LogAsync("This goes to the logger created above");
    }

    public static async Task InjectAndLogAsync()
    {
        var services = new ServiceCollection();

        var logger = new LoggerFactory()
            .CreateLogger<MsLoggerDemo>();

        services.AddPvWayMsLoggerServices(
            ServiceLifetime.Transient, logger);

        var sp = services.BuildServiceProvider();

        var msLs = sp.GetService<IPvWayMsLoggerService>()!;

        await msLs.LogAsync("This goes to the logger created above");
    }

}