using PvWay.ExcelTranslationProvider.Abstractions.nc8;

namespace PvWay.ExcelTranslationProvider.nc8.Services;

public class PvWayExcelTranslationCacheConfig(TimeSpan rescanInterval) : 
    IPvWayExcelTranslationCacheConfig
{
    public TimeSpan RescanInterval { get; } = rescanInterval;
}