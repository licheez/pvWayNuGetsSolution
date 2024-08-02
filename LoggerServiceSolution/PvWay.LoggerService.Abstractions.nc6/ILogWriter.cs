namespace PvWay.LoggerService.Abstractions.nc6;

public interface ILogWriter : IDisposable, IAsyncDisposable
{
    Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc);

    void WriteLog(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc);
}