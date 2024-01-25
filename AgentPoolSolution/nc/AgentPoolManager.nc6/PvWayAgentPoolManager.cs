using Microsoft.Extensions.DependencyInjection;
using pvWay.agentPoolManager.nc6.Abstractions;
using pvWay.agentPoolManager.nc6.Impl;

namespace pvWay.agentPoolManager.nc6;

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