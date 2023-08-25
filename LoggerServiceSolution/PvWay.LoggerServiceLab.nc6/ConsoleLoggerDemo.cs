using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

public class ConsoleLoggerDemo
{
    public async Task<double> HowToUseTheConsoleLogger(
        double x)
    {
        Console.WriteLine("Hello, ConsoleLoggerService");
        Console.WriteLine("---------------------------");
        Console.WriteLine();

        var consoleLs = PvWayLoggerService.CreateConsoleLoggerService();

        try
        {
            // dividing by zero throws an exception
            return x / 0;
        }
        catch (Exception e)
        {
            await consoleLs.LogAsync(e);
            throw;
        }
    }

    public async Task AndWithDependencyInjection()
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