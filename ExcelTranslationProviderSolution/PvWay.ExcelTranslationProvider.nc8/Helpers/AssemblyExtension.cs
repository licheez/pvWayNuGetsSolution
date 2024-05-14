using System.Reflection;

namespace PvWay.ExcelTranslationProvider.nc8.Helpers;

public static class AssemblyExtension
{
    public static DateTime GetBuildDateUtc(
        this Assembly assembly)
    {
        var filePath = assembly.Location;
        var file = new FileInfo(filePath);
        return file.LastWriteTimeUtc;
    }
}