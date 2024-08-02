namespace PvWay.LoggerService.Abstractions.nc6;

public sealed class MethodResultException : Exception
{
    public MethodResultException(string message) : 
        base(message)
    {
    }

    public MethodResultException(
        string message, Exception innerException) : 
        base(message, innerException)
    {
    }
}