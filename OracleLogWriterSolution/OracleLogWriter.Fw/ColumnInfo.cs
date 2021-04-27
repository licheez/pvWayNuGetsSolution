using System.Data;

namespace pvWay.OracleLogWriter.Fw
{
    internal class ColumnInfo
    {
        public string ColumnName { get; }
        public string Type { get; }
        public int? Length { get; }
        public bool IsNullable { get; }

        public ColumnInfo(IDataRecord dr)
        {
            var iName = dr.GetOrdinal("COLUMN_NAME");
            ColumnName = dr.GetString(iName);

            var iDataType = dr.GetOrdinal("DATA_TYPE");
            Type = dr.GetString(iDataType);

            var iLength = dr.GetOrdinal("DATA_LENGTH");

            Length = dr.IsDBNull(iLength)
                ? (int?)null : dr.GetInt32(iLength);

            var iIsNullable = dr.GetOrdinal("NULLABLE");
            IsNullable = dr.GetString(iIsNullable) == "Y";
        }
    }
}