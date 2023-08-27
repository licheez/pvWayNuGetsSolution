using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal static class MuteLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        var muteLs = PvWayLoggerService.CreateMuteLoggerService();

        await muteLs.LogAsync("hearing the sound of silence");
    }

    public static async Task InjectAndLogAsync()
    {
        var services = new ServiceCollection();

        services.AddPvWayLoggerServices(ServiceLifetime.Transient);

        var sp = services.BuildServiceProvider();

        var muteLs = sp.GetService<IPvWayMuteLoggerService>()!;

        await muteLs.LogAsync("hearing the sound of silence");
    }
}