using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.MsSqlLogWriter.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal static class MsSqlLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        const string msSqlCs = "Data Source=localhost;" +
                               "Initial Catalog=NuGetDemo;" +
                               "integrated security=True;" +
                               "MultipleActiveResultSets=True;" +
                               "TrustServerCertificate=True;";

        var msSqlLs = PvWayMsSqlLoggerService.FactorLoggerService(
            async () => await Task.FromResult(msSqlCs));

        await msSqlLs.LogAsync("Hello Ms Sql Logger Service");
    }

    public static async Task InjectAndLog()
    {
        var services = new ServiceCollection();

        const string msSqlCs = "Data Source=localhost;" +
                               "Initial Catalog=NuGetDemo;" +
                               "integrated security=True;" +
                               "MultipleActiveResultSets=True;" +
                               "TrustServerCertificate=True;";

        services.AddPvWayMsSqlLogServices(
            ServiceLifetime.Transient,
            async () => await Task.FromResult(msSqlCs));

        var sp = services.BuildServiceProvider();

        var msSqlLs = sp.GetService<IPvWayMsSqlLoggerService>()!;

        await msSqlLs.LogAsync("Hello Ms Sql Logger Service");
    }
}