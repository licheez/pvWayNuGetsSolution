using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class MsLogWriter : ILogWriter
{
    private readonly ILogger _logger;

    public MsLogWriter(ILogger logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        // nop
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
        WriteLog(
            userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc);
        await Task.CompletedTask;
    }

    public void WriteLog(
        string? userId, string? companyId, string? topic,
        SeverityEnum severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        switch (severity)
        {
            case SeverityEnum.Ok:
                _logger.LogTrace(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);
                break;
            case SeverityEnum.Debug:
                _logger.LogDebug(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);
                break;
            case SeverityEnum.Info:
                _logger.LogInformation(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);
                break;
            case SeverityEnum.Warning:
                _logger.LogWarning(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);
                break;
            case SeverityEnum.Error:
                _logger.LogError(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);
                break;
            case SeverityEnum.Fatal:
                _logger.LogCritical(
                    "user:'{UserId}', " +
                    "companyId:'{CompanyId}', " +
                    "topic:'{Topic}', " +
                    "machineName:'{MachineName}', " +
                    "memberName:'{MemberName}', " +
                    "filePath:'{FilePath}', " +
                    "lineNumber:{LineNumber}, " +
                    "message: '{Message}', " +
                    "date: {Date:yyyy-MM-dd HH:mm:ss.fff}",
                    userId, companyId, topic,
                    machineName, memberName,
                    filePath, lineNumber,
                    message, dateUtc);

                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(severity), severity, "invalid severity");
        }
    }
}