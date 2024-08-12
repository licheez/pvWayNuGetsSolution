using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

// ReSharper disable ExplicitCallerInfoArgument
[assembly: InternalsVisibleTo("PvWay.LoggerService.Console.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.Hybrid.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.MsSql.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.Mute.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.PgSql.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.SeriConsole.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerService.UTest.nc6")]
[assembly: InternalsVisibleTo("PvWay.StackTraceConsole.nc6")]
[assembly: InternalsVisibleTo("PvWay.LoggerServiceDebug.nc6")]

namespace PvWay.LoggerService.nc6;

internal interface ILoggerServiceConfig
{
    SeverityEnu MinLevel { get; }
}

internal abstract class LoggerService :
    ILoggerService
{
    private readonly ILogWriter[] _logWriters;
    private readonly ILoggerServiceConfig _config;
    
    private string? _userId;
    private string? _companyId;
    private string? _topic;

    protected LoggerService(
        ILoggerServiceConfig config,
        params ILogWriter[] logWriters)
    {
        _logWriters = logWriters;
        _config = config;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Create a log message using the formatter function
        var logMessage = formatter(state, exception);

        if (exception != null)
        {
            // capture the stack trace skipping some technical lines
            var stackTraceLines = Environment.StackTrace
                .Split(Environment.NewLine)
                .Where(x => !(
                    x.Contains("Microsoft.Extensions.Logging.LoggerExtensions")
                    || x.Contains("PvWay.LoggerService.nc")
                    || x.Contains("System.Environment")
                ));
            var sb = new StringBuilder();
            foreach (var line in stackTraceLines)
            {
                if (sb.Length > 0) sb.Append(Environment.NewLine);
                sb.Append(line);
            }

            logMessage += Environment.NewLine + sb;
        }

        // Determine the severity from the log level
        var severity = GetSeverity(logLevel);

        // find the first numbered line out of the stack trace
        // skipping the first 3 lines.
        // This represents the calling member
        var stackTrace = new StackTrace(true);
        var frame = stackTrace.GetFrames()
            .Skip(3).FirstOrDefault(x => x.GetFileLineNumber() > 0);
        var memberName = frame?.GetMethod()?.ToString() ?? string.Empty;
        var filePath = frame?.GetFileName() ?? string.Empty;
        var lineNumber = frame?.GetFileLineNumber() ?? -1;

        // Use common method to log the message
        var eventMessage = string.Empty;
        if (eventId.Id != 0 || !string.IsNullOrEmpty(eventId.Name))
        {
            eventMessage = $"[{eventId.Id}:{eventId.Name}] ";
        }
        var message = $"{eventMessage}{logMessage}";
        Log(message, severity, memberName, filePath, lineNumber);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        var severity = GetSeverity(logLevel);
        return severity >= _config.MinLevel;
    }

#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
    {
        return this;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foreach (var logWriter in _logWriters)
        {
            logWriter.Dispose();
        }
    }

    ~LoggerService()
    {
        Dispose(false);
    }

    public ValueTask DisposeAsync()
    {
        foreach (var logWriter in _logWriters)
        {
            logWriter.DisposeAsync();
        }
        GC.SuppressFinalize(this);
        return new ValueTask();
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
        if (severity < _config.MinLevel)
            return;

        foreach (var logWriter in _logWriters)
        {
            logWriter.WriteLog(
                _userId, _companyId, topic,
                severity,
                Environment.MachineName,
                memberName, filePath, lineNumber,
                message, DateTime.UtcNow);
        }
    }

    private async Task WriteLogAsync(
        string message,
        string? topic,
        SeverityEnu severity = SeverityEnu.Debug,
        string memberName = "",
        string filePath = "",
        int lineNumber = -1)
    {
        if (severity < _config.MinLevel)
            return;

        foreach (var logWriter in _logWriters)
        {
            await logWriter.WriteLogAsync(
                _userId, _companyId, topic,
                severity,
                Environment.MachineName,
                memberName, filePath, lineNumber,
                message, DateTime.UtcNow); 
        }
    }

    private static SeverityEnu GetSeverity(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
                return SeverityEnu.Trace;
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

// ReSharper disable once UnusedTypeParameter
internal abstract class LoggerService<T>: LoggerService
{
    protected LoggerService(
        ILoggerServiceConfig config,
        params ILogWriter[] logWriters): base(config, logWriters)
    {
    }
}

