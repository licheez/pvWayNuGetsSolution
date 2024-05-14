using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayAssemblyBasedExcelTranslationDi
{
    /// <summary>
    /// Creates an instance of PvWayAssemblyExcelTranslationService for managing a cached translation dictionary at the data consumer side, based on an assembly and a regular expression.
    /// </summary>
    /// <param name="assembly">The assembly from which to retrieve the translation resources.</param>
    /// <param name="resourceNameMatcher">A regular expression used to match the names of the translation resources.</param>
    /// <param name="logException">An action to handle any exceptions that occur during the translation process.</param>
    /// <returns>An instance of PvWayAssemblyExcelTranslationService.</returns>
    public static IPvWayExcelTranslationService CreateAssemblyBasedExcelTranslationService(
            Assembly assembly,
            Regex resourceNameMatcher,
            Action<Exception> logException)
    {
        var config = new PvWayAssemblyBasedExcelTranslationServiceConfig(
            assembly, resourceNameMatcher, logException);
        return new PvWayAssemblyExcelTranslationService(config);
    }

    /// <summary>
    /// Adds an assembly based excel translation service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the translation service to.</param>
    /// <param name="assembly">The assembly from which to retrieve the translation resources.</param>
    /// <param name="resourceNameMatcher">A regular expression used to match the names of the translation resources.</param>
    /// <param name="logException">An action to handle any exceptions that occur during the translation process.</param>
    /// <param name="lifetime">The lifetime of the translation service.</param>
    public static void AddAssemblyBasedExcelTranslation(
        this IServiceCollection services,
        Assembly assembly,
        Regex resourceNameMatcher,
        Action<Exception> logException,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayAssemblyBasedExcelTranslationServiceConfig>(sp =>
            new PvWayAssemblyBasedExcelTranslationServiceConfig(
                assembly, resourceNameMatcher, logException));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationService),
            typeof(PvWayAssemblyExcelTranslationService), lifetime);
        services.Add(sd);
    }
}