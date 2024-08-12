using System.Globalization;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.Console.nc8;

internal sealed class ConsoleLogWriter : IConsoleLogWriter
{
    public void Dispose()
    {
        // nop
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        await Task.Run(() =>
        {
            WriteLog(userId, companyId, topic,
                severity, machineName, memberName,
                filePath, lineNumber, message, dateUtc);
        });
    }

    public void WriteLog(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        var curColor = System.Console.ForegroundColor;
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

        System.Console.ForegroundColor = msgColor;
        System.Console.WriteLine(line);
        System.Console.ForegroundColor = curColor;
    }

}