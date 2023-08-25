using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class ConsoleLogWriter : ILogWriter
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        SeverityEnum severity, string machineName,
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
        SeverityEnum severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        var curColor = Console.ForegroundColor;
        var msgColor = severity switch
        {
            SeverityEnum.Debug => ConsoleColor.White,
            SeverityEnum.Info => ConsoleColor.DarkCyan,
            SeverityEnum.Warning => ConsoleColor.DarkYellow,
            SeverityEnum.Error => ConsoleColor.DarkRed,
            SeverityEnum.Fatal => ConsoleColor.Red,
            SeverityEnum.Ok => ConsoleColor.Green,
            _ => throw new ArgumentOutOfRangeException(
                nameof(severity), severity, "invalid severity")
        };

        var logLevel = EnumSeverity.GetMsLogLevel(severity);
        var abbrev = EnumSeverity.GetMsLogLevelAbbrev(logLevel);

        var line =
            $"{abbrev}{dateUtc} " +
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