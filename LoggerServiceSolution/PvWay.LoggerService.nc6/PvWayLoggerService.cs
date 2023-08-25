using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

// ReSharper disable UnusedMember.Global

namespace PvWay.LoggerService.nc6;

public static class PvWayLoggerService
{
    public static ILoggerService CreateConsoleLoggerService()
    {
        return new ConsoleLogger();
    }

    public static ILoggerService CreateMuteLoggerService()
    {
        return new MuteLogger();
    }

    public static ILoggerService CreateMsLoggerService(ILogger msLogger)
    {
        return new MsLogger(msLogger);
    }

    public static ILoggerService CreateMsConsoleLoggerService()
    {
        return new MsConsoleLogger();
    }

    public static ILoggerService CreateMultiChannelLoggerService(
        IEnumerable<ILoggerService> loggerServices)
    {
        return new MultiChannelLogger(loggerServices);
    }

    public static void AddPvWayLoggerServices(
        this ServiceCollection services,
        ServiceLifetime lifetime)
    {
        var consoleLoggerSd = new ServiceDescriptor(
            typeof(IPvWayConsoleLoggerService),
            _ => new ConsoleLogger(),
            lifetime);
        services.Add(consoleLoggerSd);

        var muteLoggerSd = new ServiceDescriptor(
            typeof(IPvWayMuteLoggerService),
            _ => new MuteLogger(),
            lifetime);
        services.Add(muteLoggerSd);

        var msConsoleLoggerSd = new ServiceDescriptor(
            typeof(IPvWayMsConsoleLoggerService),
            _ => new MsConsoleLogger(),
            lifetime);
        services.Add(msConsoleLoggerSd);
    }

    public static void AddPvWayMsLoggerServices(
        this ServiceCollection services,
        ServiceLifetime lifetime,
        ILogger logger)
    {
        var msLoggerSd = new ServiceDescriptor(
            typeof(IPvWayMsLoggerService),
            _ => new MsLogger(logger),
            lifetime);
        services.Add(msLoggerSd);
    }

    public static void AddPvWayMultiChannelServices(
        this ServiceCollection services,
        ServiceLifetime lifetime,
        IEnumerable<ILoggerService> loggerServices)
    {
        var sd = new ServiceDescriptor(
            typeof(IPvWayMultiChannelLoggerService),
            _ => new MultiChannelLogger(loggerServices),
            lifetime);
        services.Add(sd);
    }
}