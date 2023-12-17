namespace PvWay.MsSqlSemaphoreService.nc8.Helpers;

internal static class DaoHelper
{
    public static string TruncateThenEscape(string value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var val = value.Length <= maxLen
            ? value
            : value[..(maxLen - 3)] + "...";
        return Escape(val);
    }

    private static string Escape(string value)
    {
        return value.Replace("'", "''");
    }

    /// <summary>
    /// returns a ready to concat sql string in
    /// the form 'yyyy-MM-dd HH:mm:ss.sss'
    /// </summary>
    /// <param name="utcNow"></param>
    /// <returns></returns>
    public static string GetTimestamp(DateTime utcNow) => $"'{utcNow:yyyy-MM-dd HH:mm:ss.sss}'";
    
}