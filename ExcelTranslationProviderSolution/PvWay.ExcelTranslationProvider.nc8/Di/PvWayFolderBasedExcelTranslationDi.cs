using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayFolderBasedExcelTranslationDi
{
    /// <summary>
    /// Creates an instance of the PvWayFolderBasedExcelTranslationService class,
    /// which implements the IPvWayExcelTranslationService interface.
    /// </summary>
    /// <param name="excelFolderPath">The path to the folder that contains the Excel translation files.</param>
    /// <param name="fileWildCard">The wild card for the Excel translation file names.</param>
    /// <param name="logException">The action to be executed when an exception occurs.</param>
    /// <returns>An instance of the PvWayFolderBasedExcelTranslationService class.</returns>
    public static IPvWayExcelTranslationService CreateFolderBasedExcelTranslation(
            string excelFolderPath, 
            string fileWildCard, 
            Action<Exception> logException)
    {
        var config = new PvWayFolderBasedExcelTranslationServiceConfig(
            excelFolderPath, fileWildCard, logException);
        return new PvWayFolderBasedExcelTranslationService(config);
    }

    /// <summary>
    /// Adds the PvWayFolderBasedExcelTranslationService to the IServiceCollection
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to</param>
    /// <param name="excelFolderPath">The path to the folder that contains the Excel translation files</param>
    /// <param name="fileWildCard">The wild card for the Excel translation file names</param>
    /// <param name="logException">The action to be executed when an exception occurs</param>
    /// <param name="lifetime">The lifetime of the service (default is Singleton)</param>
    public static void AddPvWayFolderBasedExcelTranslation(
        this IServiceCollection services,
        string excelFolderPath, 
        string fileWildCard, 
        Action<Exception> logException, 
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayFolderBasedExcelTranslationServiceConfig>(_ =>
            new PvWayFolderBasedExcelTranslationServiceConfig(excelFolderPath, fileWildCard, logException));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationService),
            typeof(PvWayFolderBasedExcelTranslationService), lifetime);
        services.Add(sd);
    }
}