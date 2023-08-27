using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal static class MsConsoleLoggerDemo
{
    public static async Task FactorAndLog()
    {
        var msConsoleLs = PvWayLoggerService.CreateMsConsoleLoggerService();
        var e = new Exception("Some exception");
        await msConsoleLs.LogAsync(e);

        await msConsoleLs.LogAsync("This is ok", SeverityEnum.Ok);
        await msConsoleLs.LogAsync("This is debug");
        await msConsoleLs.LogAsync("This is an info", SeverityEnum.Info);
        await msConsoleLs.LogAsync("This is a warning", SeverityEnum.Warning);
        await msConsoleLs.LogAsync("This is an error", SeverityEnum.Error);
        await msConsoleLs.LogAsync("This is a fatal", SeverityEnum.Fatal);
    }

    public static async Task InjectAndLog()
    {
        var services = new ServiceCollection();

        services.AddPvWayLoggerServices(ServiceLifetime.Transient);

        var sp = services.BuildServiceProvider();

        var msConsoleLs = sp.GetService<IPvWayMsConsoleLoggerService>()!;

        await msConsoleLs.LogAsync("This goes to the Microsoft Console Logger");
    }
}