using Microsoft.Extensions.DependencyInjection;
using PvWay.AgentPoolManager.Abstraction.nc8;
using PvWay.AgentPoolManager.nc8.Impl;

namespace PvWay.AgentPoolManager.nc8;

public static class PvWayAgentPoolManager
{
    public static void AddPvWayAgentPoolManager(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var sd = new ServiceDescriptor(
            typeof(IPvWayAgentPoolManager), 
            typeof(PoolManager),
            lifetime);
        services.Add(sd);
    }

    public static IPvWayAgentPoolManager Create() => new PoolManager();
}