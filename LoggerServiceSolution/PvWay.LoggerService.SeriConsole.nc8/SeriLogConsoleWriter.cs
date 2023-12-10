using System.Globalization;
using PvWay.LoggerService.Abstractions.nc8;
using Serilog;
using Serilog.Events;

namespace PvWay.LoggerService.SeriConsole.nc8;

internal sealed class SeriLogConsoleWriter: ILogWriter
{
    private readonly ILogger _sLogger = new LoggerConfiguration()
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .CreateLogger();
    
    public void Dispose()
    {
        // nop
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public Task WriteLogAsync(
        string? userId, string? companyId, string? topic, 
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, 
        string message, DateTime dateUtc)
    {
        WriteLog(userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc);
        return Task.CompletedTask;
    }

    public void WriteLog(string? userId, string? companyId, string? topic, 
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, 
        string message, DateTime dateUtc)
    {
        if (severity == SeverityEnu.Ok) return;
        var sLevel = GetLogEventLevel(severity);
        var userIdStr = string.IsNullOrEmpty(userId)
            ? string.Empty
            : $" userId: '{userId}'";
        var companyIdStr = string.IsNullOrEmpty(companyId)
            ? string.Empty
            : $" companyId: '{companyId}'";
        var topicStr = string.IsNullOrEmpty(topic)
            ? string.Empty
            : $" topic: '{topic}'";
        var dateUtcStr = dateUtc.ToString(CultureInfo.InvariantCulture);
        _sLogger.Write(sLevel,
            "{Message} from {MachineName} in {MemberName} " +
            "({FilePath}) line {LineNumber} at {DateUtc}" +
            "{UserId}{CompanyId}{Topic}",
            machineName,
            message, memberName, filePath, 
            lineNumber, dateUtcStr,
            userIdStr, companyIdStr, topicStr);
    }

    private static LogEventLevel GetLogEventLevel(SeverityEnu severity)
    {
        return severity switch
        {
            SeverityEnu.Ok => LogEventLevel.Debug,
            SeverityEnu.Debug => LogEventLevel.Debug,
            SeverityEnu.Info => LogEventLevel.Information,
            SeverityEnu.Warning => LogEventLevel.Warning,
            SeverityEnu.Error => LogEventLevel.Error,
            SeverityEnu.Fatal => LogEventLevel.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }
}