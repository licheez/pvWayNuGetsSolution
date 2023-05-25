using Microsoft.Extensions.DependencyInjection;
using PvWayDaoAbstractions;

namespace PvWayMsSqlDao
{
    public static class MsSqlDaoServiceDi
    {
        // ReSharper disable once UnusedMember.Global
        public static void AddPvWayMsSqlDaoService(
            this IServiceCollection services,
            Func<Exception, Task> logAsync,
            string connectionString)
        {
            services.AddTransient<IDaoService>(_ => 
                new MsSqlDaoService(logAsync, connectionString));
        }
    }
}
