using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class ConsoleLogWriter: ILogWriter
{
#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
        // nop
    }
#pragma warning disable CA1816

    public ValueTask DisposeAsync()
#pragma warning restore CA1816
    {
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
        Console.ForegroundColor = msgColor;
        var line =
            $"severity: '{severity}'{Environment.NewLine}" +
            $"machineName: '{machineName}'{Environment.NewLine}" +
            $"memberName: '{memberName}'{Environment.NewLine}" +
            $"filePath: '{filePath}'{Environment.NewLine}" +
            $"lineNumber: '{lineNumber}'{Environment.NewLine}" +
            $"topic: '{topic}'{Environment.NewLine}" +
            $"message:'{message}'{Environment.NewLine}" +
            $"user: '{userId}'{Environment.NewLine}" +
            $"company: '{companyId}'{Environment.NewLine}" +
            $"dateUtc : '{dateUtc}'{Environment.NewLine}" +
            $"{Environment.NewLine}";
        Console.WriteLine(line);
        Console.ForegroundColor = curColor;
    }

}