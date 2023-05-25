using PvWayDaoAbstractions;
using System.Data;

namespace PvWayMsSqlDao;

internal class MsSqlDaoReader : IDaoReader
{
    private readonly IDataReader _r;
    public MsSqlDaoReader(IDataReader r)
    {
        _r = r;
    }

    public string GetString(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        if (isNull) throw new ArgumentException(
            $"value for {name} should not be null");
        return _r.GetString(i);
    }

    public string? GetStringOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetString(i);
    }

    public int GetInt(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        if (isNull) throw new ArgumentException(
            $"value for {name} should not be null");
        return _r.GetInt32(i);
    }

    public int? GetIntOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetInt32(i);
    }

    public decimal? GetDecimalOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetDecimal(i);
    }

    public double? GetDoubleOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetDouble(i);
    }

    public long GetLong(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        if (isNull) throw new ArgumentException(
            $"value for {name} should not be null");
        return _r.GetInt64(i);
    }

    public long? GetLongOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetInt64(i);
    }

    public DateTime GetDate(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        if (isNull) throw new ArgumentException(
            $"value for {name} should not be null");
        return _r.GetDateTime(i);
    }

    public DateTime? GetDateOrNull(string name)
    {
        var i = _r.GetOrdinal(name);
        var isNull = _r.IsDBNull(i);
        return isNull ? null : _r.GetDateTime(i);
    }
}