using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.Mute.nc8;

internal sealed class MuteLogWriter: IMuteLogWriter
{
    public void Dispose()
    {
        // nop
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public Task WriteLogAsync(string? userId, string? companyId, string? topic, 
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, string message, DateTime dateUtc)
    {
        return Task.CompletedTask;
    }

    public void WriteLog(string? userId, string? companyId, string? topic, 
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber, string message, DateTime dateUtc)
    {
        // nop
    }
}