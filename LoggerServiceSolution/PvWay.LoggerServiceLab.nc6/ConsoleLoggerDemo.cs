using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

public static class ConsoleLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        var consoleLs = PvWayLoggerService.CreateConsoleLoggerService();

        var e = new Exception("Some exception");
        await consoleLs.LogAsync(e);

        await consoleLs.LogAsync("This is ok", SeverityEnum.Ok);
        await consoleLs.LogAsync("This is debug");
        await consoleLs.LogAsync("This is an info", SeverityEnum.Info);
        await consoleLs.LogAsync("This is a warning", SeverityEnum.Warning);
        await consoleLs.LogAsync("This is an error", SeverityEnum.Error);
        await consoleLs.LogAsync("This is a fatal", SeverityEnum.Fatal);
    }

    public static async Task InjectAndLogAsync()
    {
        var services = new ServiceCollection();

        // provisions the different loggerServices
        // ConsoleLogger, MuteLogger, MsConsoleLogger...
        services.AddPvWayLoggerServices(ServiceLifetime.Transient);

        var sp = services.BuildServiceProvider();

        // Retrieve the ConsoleLogger
        var consoleLs = sp.GetService<IPvWayConsoleLoggerService>()!;

        // Use it
        await consoleLs.LogAsync("Not that complex after all");
    }

}