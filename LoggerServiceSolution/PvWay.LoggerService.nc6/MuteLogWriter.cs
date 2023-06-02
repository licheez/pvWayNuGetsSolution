using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class MuteLogWriter : ILogWriter
{
    public void Dispose()
    {
        // nop
    }
    public ValueTask DisposeAsync()
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
        // nop
    }

}