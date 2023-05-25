namespace PvWayDaoAbstractions;

public interface IDaoReader
{
    string GetString(string name);
    string? GetStringOrNull(string name);
    int GetInt(string name);
    int? GetIntOrNull(string name);
    decimal? GetDecimalOrNull(string name);
    long GetLong(string name);
    long? GetLongOrNull(string name);
    DateTime GetDate(string name);
    DateTime? GetDateOrNull(string name);
    double? GetDoubleOrNull(string name);
}