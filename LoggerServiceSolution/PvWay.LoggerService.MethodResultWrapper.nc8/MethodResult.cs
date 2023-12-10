using System.Text;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MethodResultWrapper.nc8;

public class MethodResult : IMethodResult
{
    private readonly IList<IMethodResultNotification> _notifications;

    private sealed class Notification(
        SeverityEnu severity, string message) : IMethodResultNotification
    {
        public SeverityEnu Severity { get; } = severity;
        public string Message { get; } = message;

        public override string ToString()
        {
            return $"{Severity}:{Message}";
        }
    }

    /// <summary>
    /// Successful constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public MethodResult()
    {
        _notifications = new List<IMethodResultNotification>();
    }

    /// <summary>
    /// Wraps the result of a previous MethodResult
    /// by copying all its notifications (message and severity).
    /// Not really useful for the non generic MethodResult class
    /// but makes sense when using the generic version of the class
    /// </summary>
    /// <param name="res"></param>
    // ReSharper disable once MemberCanBeProtected.Global
    public MethodResult(IMethodResult res)
        : this()
    {
        foreach (var notification in res.Notifications)
        {
            AddNotification(notification);
        }
    }

    /// <summary>
    /// Instantiates a MethodResult object with one
    /// notification (message and severity)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    // ReSharper disable once MemberCanBeProtected.Global
    public MethodResult(string message, SeverityEnu severity) :
        this()
    {
        AddNotification(message, severity);
    }

    /// <summary>
    /// Failure constructor that instantiates a MethodResult
    /// object with one notification. The notification message
    /// is built by recursively walking down the exception
    /// and its inner exceptions (using the extension method
    /// e.GetDeepMessage()). The stack trace is also added at
    /// the end of the message. By default the Severity is
    /// considered as Fatal but you select the one of your choice
    /// </summary>
    /// <param name="e"></param>
    /// <param name="severity"></param>
    public MethodResult(Exception e, SeverityEnu severity = SeverityEnu.Fatal)
        : this(e.GetDeepMessage(), severity)
    {
    }

    /// <summary>
    /// Same as the single message constructor but this time
    /// passing a list of messages. This creates one notification
    /// for each message in the list. All notifications get the
    /// same severity
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="severity"></param>
    // ReSharper disable once MemberCanBeProtected.Global
    public MethodResult(IEnumerable<string> messages, SeverityEnu severity) :
        this()
    {
        foreach (var message in messages)
        {
            AddNotification(message, severity);
        }
    }

    public void AddNotification(string message, SeverityEnu severity)
    {
        AddNotification(new Notification(severity, message));
    }

    public void AddNotification(IMethodResultNotification notification)
    {
        _notifications.Add(notification);
    }

    /// <summary>
    /// At least one notification has a severity
    /// greater or equal to Error
    /// </summary>
    public bool Failure => _notifications
        .Any(n => n.Severity >= SeverityEnu.Error);

    /// <summary>
    /// No notification or all notifications severity
    /// are lower than Error
    /// </summary>
    public bool Success => !Failure;

    /// <summary>
    /// returns the highest severity from the list of notifications
    /// </summary>
    public SeverityEnu Severity =>
        _notifications.Any()
            ? _notifications.Max(x => x.Severity)
            : SeverityEnu.Ok;

    public IEnumerable<IMethodResultNotification> Notifications =>
        _notifications;

    public void Throw()
    {
        throw new MethodResultException(ErrorMessage);
    }

    public string ErrorMessage => ToString();

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var notification in _notifications)
        {
            if (sb.Length > 0) 
                sb.Append(Environment.NewLine);
            sb.Append(notification);
        }
        return sb.ToString();
    }

    public static MethodResult Ok => new();
        
}

public class MethodResult<T> : MethodResult, IMethodResult<T>
{
    public T? Data { get; }

    public static IMethodResult<T> Null => new MethodResult<T>(default(T));

    /// <summary>
    /// Successful constructor that carries
    /// the result of the called method in the
    /// property Data of type T
    /// </summary>
    /// <param name="data"></param>
    public MethodResult(T? data)
    {
        Data = data;
    }

    /// <summary>
    /// Wraps the result of a previous MethodResult
    /// by copying all its notifications (message and severity).
    /// Not really useful for the non generic MethodResult class
    /// but makes sense when using the generic version of the class
    /// </summary>
    /// <param name="methodResult"></param>
    public MethodResult(IMethodResult methodResult) :
        base(methodResult)
    {
    }

    /// <summary>
    /// Instantiates a MethodResult object with one
    /// notification (message and severity)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    public MethodResult(string message, SeverityEnu severity)
        : base(message, severity)
    {
    }

    /// <summary>
    /// Same as the single message constructor but this time
    /// passing a list of messages. This creates one notification
    /// for each message in the list. All notifications get the
    /// same severity
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="severity"></param>
    public MethodResult(IEnumerable<string> messages, SeverityEnu severity)
        : base(messages, severity)
    {
    }

    /// <summary>
    /// Failure constructor that instantiates a MethodResult
    /// object with one notification. The notification message
    /// is built by recursively walking down the exception
    /// and its inner exceptions (using the extension method
    /// e.GetDeepMessage()). The stack trace is also added at
    /// the end of the message. By default the Severity is
    /// considered as Fatal but you select the one of your choice
    /// </summary>
    /// <param name="e"></param>
    /// <param name="severity"></param>
    public MethodResult(Exception e, SeverityEnu severity = SeverityEnu.Fatal)
        : base(e, severity)
    {
    }

}