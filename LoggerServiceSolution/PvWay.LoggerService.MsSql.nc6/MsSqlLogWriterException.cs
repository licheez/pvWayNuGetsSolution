namespace PvWay.LoggerService.MsSql.nc6;

public sealed class MsSqlLogWriterException: Exception
{
    public MsSqlLogWriterException(string? message) : 
        base(message)
    {
    }

    public MsSqlLogWriterException(
        string? message, Exception? innerException) : 
        base(message, innerException)
    {
    }
}