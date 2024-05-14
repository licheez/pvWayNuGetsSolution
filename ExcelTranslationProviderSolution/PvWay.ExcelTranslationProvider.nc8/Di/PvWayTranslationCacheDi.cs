using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayTranslationCacheDi
{
    /// <summary>
    /// Creates an instance of IPvWayExcelTranslationCache.
    /// </summary>
    /// <param name="ts">The implementation of IPvWayExcelTranslationService.</param>
    /// <param name="rescanInterval">
    /// The time interval for re-scanning the folder for updated Excel files.
    /// set it to TimeSpan.Zero for disabling the rescan
    /// </param>
    /// <returns>An instance of IPvWayExcelTranslationCache.</returns>
    public static IPvWayExcelTranslationCache CreateTranslationCache(
            IPvWayExcelTranslationService ts,
            TimeSpan rescanInterval)
    {
        var config = new PvWayExcelTranslationCacheConfig(rescanInterval);
        return new PvWayExcelTranslationCache(ts, config);
    }

    /// <summary>
    /// Adds the PvWayTranslationCache to the ServiceCollection with the specified rescan interval and service lifetime.
    /// </summary>
    /// <param name="services">The ServiceCollection to add the PvWayTranslationCache to.</param>
    /// <param name="rescanInterval">
    /// The time interval for re-scanning the folder for updated Excel files.
    /// set it to TimeSpan.Zero for disabling the rescan
    /// </param>
    /// <param name="lifetime">The service lifetime (default: Singleton).</param>
    public static void AddPvWayTranslationCache(
        this ServiceCollection services,
        TimeSpan rescanInterval,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayExcelTranslationCacheConfig>(_ =>
            new PvWayExcelTranslationCacheConfig(rescanInterval));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationCache),
            typeof(PvWayExcelTranslationCache),
            lifetime);
        services.Add(sd);
    } 
}