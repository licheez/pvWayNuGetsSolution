using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.StackTraceConsole.nc8;

public sealed class StackTraceLogWriter: ILogWriter
{
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
        WriteLog(userId, companyId, topic, severity,
            memberName, memberName, filePath, lineNumber, 
            message, dateUtc);
        return Task.CompletedTask;
    }

    public void WriteLog(
        string? userId, string? companyId, string? topic, 
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, 
        string message, DateTime dateUtc)
    {
        var nl = Environment.NewLine;
        Console.WriteLine(
            $"userId: '{userId}'{nl}" +
            $"companyId: '{companyId}'{nl}" +
            $"topic: '{topic}'{nl}" +
            $"severity: '{severity}'{nl}" +
            $"machineName: '{machineName}'{nl}" +
            $"memberName: '{memberName}'{nl}" +
            $"filePath: '{filePath}'{nl}" +
            $"lineNumber: {lineNumber}{nl}" +
            $"message: '{message}'{nl}" +
            $"dateUtc: {dateUtc:f}" +
            $"{Environment.NewLine}");
    }
}