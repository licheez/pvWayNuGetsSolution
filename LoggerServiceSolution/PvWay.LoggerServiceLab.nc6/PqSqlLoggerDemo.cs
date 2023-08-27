using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.PgSqlLogWriter.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal static class PqSqlLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        const string pgSqlCs = "Server=localhost;" +
                               "Database=postgres;" +
                               "User Id=sa;" +
                               "Password=S0mePwd_;";

        var pgSqlLs = PvWayPgLoggerService.CreateLoggerService(
            async () => await Task.FromResult(pgSqlCs));

        await pgSqlLs.LogAsync("Hello PostgreSQL Logger Service");
    }

    public static async Task InjectAndLogAsync()
    {
        const string pgSqlCs = "Server=localhost;" +
                               "Database=postgres;" +
                               "User Id=sa;" +
                               "Password=S0mePwd_;";

        var services = new ServiceCollection();

        services.AddPvWayPgLogServices(
            ServiceLifetime.Transient,
            async () =>
                await Task.FromResult(pgSqlCs));
        var sp = services.BuildServiceProvider();

        var pgSqlLs = sp.GetService<IPvWayPostgreLoggerService>()!;

        await pgSqlLs.LogAsync("Hello PostgreSQL Logger Service");
    }
}