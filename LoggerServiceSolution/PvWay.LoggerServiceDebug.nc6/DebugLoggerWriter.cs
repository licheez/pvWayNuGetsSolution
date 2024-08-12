using System.Globalization;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerServiceDebug.nc6;

internal sealed class DebugLoggerWriter: ILogWriter
{
    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public async Task WriteLogAsync(string? userId, string? companyId, string? topic, SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, string message, DateTime dateUtc)
    {
        await Task.Run(() =>
        {
            WriteLog(userId, companyId, topic,
                severity, machineName, memberName,
                filePath, lineNumber, message, dateUtc);
        });
    }

    public void WriteLog(string? userId, string? companyId, string? topic, SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, string message, DateTime dateUtc)
    {
        var curColor = Console.ForegroundColor;
        var msgColor = severity switch
        {
            SeverityEnu.Trace => ConsoleColor.Gray,
            SeverityEnu.Debug => ConsoleColor.White,
            SeverityEnu.Info => ConsoleColor.DarkCyan,
            SeverityEnu.Warning => ConsoleColor.DarkYellow,
            SeverityEnu.Error => ConsoleColor.DarkRed,
            SeverityEnu.Fatal => ConsoleColor.Red,
            SeverityEnu.Ok => ConsoleColor.Green,
            _ => throw new ArgumentOutOfRangeException(
                nameof(severity), severity, "invalid severity")
        };

        var logLevel = EnumSeverity.GetMsLogLevel(severity);
        var abbrev = EnumSeverity.GetMsLogLevelAbbrev(logLevel);

        var dt = dateUtc.ToString(CultureInfo.InvariantCulture);
        
        var line =
            $"{abbrev}{dt} " +
            $"{machineName}.{memberName}.{lineNumber}" +
            $"{Environment.NewLine}    '{message}'";

        if (!string.IsNullOrEmpty(topic))
            line += $"{Environment.NewLine}    topic: '{topic}'";
        
        if (!string.IsNullOrEmpty(userId))
            line += $"{Environment.NewLine}    user: '{userId}'";

        if (!string.IsNullOrEmpty(companyId))
            line += $"{Environment.NewLine}    company: '{companyId}'";

        line += Environment.NewLine;

        Console.ForegroundColor = msgColor;
        Console.WriteLine(line);
        Console.ForegroundColor = curColor;
    }
}