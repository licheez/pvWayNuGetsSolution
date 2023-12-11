namespace PvWay.LoggerService.PgSql.nc8;

public sealed class PgSqlLogWriterException: Exception
{
    public PgSqlLogWriterException(string? message) : 
        base(message)
    {
    }

    public PgSqlLogWriterException(
        string? message, Exception? innerException) : 
        base(message, innerException)
    {
    }
}