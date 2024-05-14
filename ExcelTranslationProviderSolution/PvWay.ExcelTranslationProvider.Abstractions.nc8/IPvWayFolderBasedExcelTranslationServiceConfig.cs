namespace PvWay.ExcelTranslationProvider.Abstractions.nc8;

public interface IPvWayFolderBasedExcelTranslationServiceConfig
{
    /// <summary>
    /// The external folder containing the translation Excel 
    /// </summary>
    string ExcelFolderPath { get; }
    /// <summary>
    /// A wildcard such as "tr_*.xls?"
    /// </summary>
    string FileWildCard { get; }
    /// <summary>
    /// A delegate for logging exceptions
    /// </summary>
    Action<Exception> LogException { get; }
}