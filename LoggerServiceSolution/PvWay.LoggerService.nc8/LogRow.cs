using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.nc8;

internal class LogRow(
    int id,
    string? userId,
    string? companyId,
    string? topic,
    SeverityEnu severity,
    string machineName,
    string memberName,
    string filePath,
    int lineNumber,
    string message,
    DateTime creationDateUtc)
    : ILoggerServiceRow
{
    public int Id { get; } = id;
    public string? UserId { get; } = userId;
    public string? CompanyId { get; } = companyId;
    public string? Topic { get; } = topic;
    public SeverityEnu Severity { get; } = severity;
    public string MachineName { get; } = machineName;
    public string MemberName { get; } = memberName;
    public string FilePath { get; } = filePath;
    public int LineNumber { get; } = lineNumber;
    public string Message { get; } = message;
    public DateTime CreationDateUtc { get; } = creationDateUtc;
}