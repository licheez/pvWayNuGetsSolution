using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;

[assembly: InternalsVisibleTo("PvWay.LoggerService.Console.nc8")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.SeriConsole.nc8")]
namespace PvWay.LoggerService.nc8;

internal abstract class LoggerService(
    ILogWriter logWriter,
    SeverityEnu minLevel = SeverityEnu.Info) : 
    ILoggerService
{
    private string? _userId;
    private string? _companyId;
    private string? _topic;

    public void Log<TState>(
        LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        // Create a log message using the formatter function
        var logMessage = formatter(state, exception);
    
        // Determine the severity from the log level
        var severity = GetSeverity(logLevel);
    
        // Use common method to log the message
        Log(logMessage, severity);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        var severity = GetSeverity(logLevel);
        return severity >= minLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Dispose()
    {
        logWriter.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return logWriter.DisposeAsync();
    }

    public void SetUser(string? userId, string? companyId = null)
    {
        _userId = userId;
        _companyId = companyId;
    }

    public void SetTopic(string? topic)
    {
        _topic = topic;
    }

    public void Log(
        string message,
        SeverityEnu severity = SeverityEnu.Debug,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(message, _topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IEnumerable<string> messages,
        SeverityEnu severity,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(GetMessage(messages), _topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        Exception e,
        SeverityEnu severity = SeverityEnu.Fatal,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(e.GetDeepMessage(), _topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IMethodResult result,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(result.ErrorMessage, _topic, result.Severity,
            memberName, filePath, lineNumber);
    }
    
    public void Log(
        string message,
        string? topic,
        SeverityEnu severity = SeverityEnu.Debug,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(message, topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IMethodResult result,
        string? topic,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(result.ErrorMessage, topic, result.Severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IEnumerable<string> messages,
        string? topic,
        SeverityEnu severity,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(GetMessage(messages), topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        Exception e,
        string? topic,
        SeverityEnu severity = SeverityEnu.Fatal,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        WriteLog(e.GetDeepMessage(), topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        string message,
        SeverityEnu severity = SeverityEnu.Debug,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(message, _topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        IEnumerable<string> messages,
        SeverityEnu severity,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(GetMessage(messages), _topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        Exception e,
        SeverityEnu severity = SeverityEnu.Fatal,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(e.GetDeepMessage(), _topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        IMethodResult result,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(result.ErrorMessage, _topic, result.Severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        string message,
        string? topic,
        SeverityEnu severity = SeverityEnu.Debug,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(message, topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        IEnumerable<string> messages,
        string? topic,
        SeverityEnu severity,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(GetMessage(messages), topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        Exception e,
        string? topic,
        SeverityEnu severity = SeverityEnu.Fatal,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(e.GetDeepMessage(), topic, severity,
            memberName, filePath, lineNumber);
    }

    public Task LogAsync(
        IMethodResult result,
        string? topic,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = -1)
    {
        return WriteLogAsync(result.ErrorMessage, topic, result.Severity,
            memberName, filePath, lineNumber);
    }
    
    private static string GetMessage(IEnumerable<string> messages)
    {
        var sb = new StringBuilder();
        foreach (var msg in messages)
        {
            if (sb.Length > 0)
                sb.Append(Environment.NewLine);
            sb.Append(msg);
        }
        return sb.ToString();
    }
    
    
    private void WriteLog(
        string message,
        string? topic,
        SeverityEnu severity = SeverityEnu.Debug,
        string memberName = "",
        string filePath = "",
        int lineNumber = -1)
    {
        if (severity < minLevel) 
            return;
        
        logWriter.WriteLog(
            _userId, _companyId, topic,
            severity,
            Environment.MachineName,
            memberName, filePath, lineNumber,
            message, DateTime.UtcNow);
    }
    
    private Task WriteLogAsync(
        string message,
        string? topic,
        SeverityEnu severity = SeverityEnu.Debug,
        string memberName = "",
        string filePath = "",
        int lineNumber = -1)
    {
        if (severity < minLevel)
            return Task.CompletedTask;
        
        return logWriter.WriteLogAsync(
            _userId, _companyId, topic,
            severity,
            Environment.MachineName,
            memberName, filePath, lineNumber,
            message, DateTime.UtcNow);
    }
    
    private static SeverityEnu GetSeverity(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                return SeverityEnu.Debug;
            case LogLevel.Information:
                return SeverityEnu.Info;
            case LogLevel.Warning:
                return SeverityEnu.Warning;
            case LogLevel.Error:
                return SeverityEnu.Error;
            case LogLevel.Critical:
                return SeverityEnu.Fatal;
            case LogLevel.None:
                return SeverityEnu.Ok;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }    

    
}