using System.Data;

namespace PvWay.PgSqlLogWriter.nc6;

internal class ColumnInfo
{
    public string ColumnName { get; }
    public string Type { get; }
    public int? Length { get; }
    public bool IsNullable { get; }

    public ColumnInfo(IDataRecord dr)
    {
        var iName = dr.GetOrdinal("column_name");
        ColumnName = dr.GetString(iName);

        var iDataType = dr.GetOrdinal("data_type");
        Type = dr.GetString(iDataType);

        var iLength = dr.GetOrdinal("character_maximum_length");

        Length = dr.IsDBNull(iLength)
            ? (int?)null : dr.GetInt32(iLength);

        var iIsNullable = dr.GetOrdinal("is_nullable");
        IsNullable = dr.GetString(iIsNullable) == "YES";
    }
}
