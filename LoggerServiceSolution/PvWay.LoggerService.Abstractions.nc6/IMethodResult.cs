namespace PvWay.LoggerService.Abstractions.nc6;

public interface IMethodResult
{
    /// <summary>
    /// At least one notification has a severity
    /// greater or equal to Error
    /// </summary>
    bool Failure { get; }

    /// <summary>
    /// No notification or all notifications severity
    /// are lower than Error
    /// </summary>
    bool Success { get; }

    SeverityEnu Severity { get; }

    /// <summary>
    /// Bulk string made of the concatenation
    /// of the notifications separated by new
    /// lines
    /// </summary>
    string ErrorMessage { get; }

    IEnumerable<IMethodResultNotification> Notifications { get; }

    void AddNotification(string message, SeverityEnu severity);
    void AddNotification(IMethodResultNotification notification);

    /// <summary>
    /// Will throw new Exception(ErrorMessage)
    /// </summary>
    void Throw();
}

public interface IMethodResult<out T> : IMethodResult
{
    T? Data { get; }
}

public interface IMethodResultNotification
{
    SeverityEnu Severity { get; }
    string Message { get; }
}