namespace PvWay.ExcelTranslationProvider.Abstractions.nc8;

public interface IPvWayExcelTranslationCacheConfig
{
    /// <summary>
    /// The cache will regularly re-scan the provided folder
    /// at the provided interval. set it to Zero if you
    /// do not want the cache to re-scan the folder
    /// </summary>
    TimeSpan RescanInterval { get; }
}