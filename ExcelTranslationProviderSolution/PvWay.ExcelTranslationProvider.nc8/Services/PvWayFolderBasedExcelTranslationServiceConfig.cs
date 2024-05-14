using PvWay.ExcelTranslationProvider.Abstractions.nc8;

namespace PvWay.ExcelTranslationProvider.nc8.Services;

internal class PvWayFolderBasedExcelTranslationServiceConfig(
    string excelFolderPath,
    string fileWildCard,
    Action<Exception> logException)
    :
        IPvWayFolderBasedExcelTranslationServiceConfig
{
    public string ExcelFolderPath { get; } = excelFolderPath;
    public string FileWildCard { get; } = fileWildCard;
    public Action<Exception> LogException { get; } = logException;
}