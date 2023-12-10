namespace PvWay.LoggerService.Abstractions.nc8;

public sealed class MethodResultException : Exception
{
    public MethodResultException()
    {
    }

    public MethodResultException(string message) : 
        base(message)
    {
    }

    public MethodResultException(string message, Exception innerException) : 
        base(message, innerException)
    {
    }
}