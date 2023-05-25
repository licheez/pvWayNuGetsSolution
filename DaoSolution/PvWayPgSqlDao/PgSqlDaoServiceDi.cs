using Microsoft.Extensions.DependencyInjection;
using PvWayDaoAbstractions;

namespace PvWayPgSqlDao;

public static class PgSqlDaoServiceDi
{
    // ReSharper disable once UnusedMember.Global
    public static void AddPvWayPgSqlDaoService(
        this IServiceCollection services,
        Func<Exception, Task> logAsync,
        string connectionString)
    {
        services.AddTransient<IDaoService>(_ =>
            new PgSqlDaoService(logAsync, connectionString));
    }

}