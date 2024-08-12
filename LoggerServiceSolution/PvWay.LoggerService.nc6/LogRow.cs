using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class LogRow: ILoggerServiceRow
{
    public int Id { get; }
    public string? UserId { get; }
    public string? CompanyId { get; }
    public string? Topic { get; }
    public SeverityEnu Severity { get; }
    public string MachineName { get; }
    public string MemberName { get; }
    public string FilePath { get; }
    public int LineNumber { get; }
    public string Message { get; }
    public DateTime CreationDateUtc { get; }

    public LogRow(
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
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        Topic = topic;
        Severity = severity;
        MachineName = machineName;
        MemberName = memberName;
        FilePath = filePath;
        LineNumber = lineNumber;
        Message = message;
        CreationDateUtc = creationDateUtc;
    }
}