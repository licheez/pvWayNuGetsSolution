namespace PvWay.LoggerService.Abstractions.nc6;

public interface ILoggerServiceRow
{
    int Id { get; }
    string? UserId { get; }
    string? CompanyId { get; }
    string? Topic { get; }
    SeverityEnu Severity { get; }
    string MachineName { get; }
    string MemberName { get; }
    string FilePath { get; }
    int LineNumber { get; }
    string Message { get; }
    DateTime CreationDateUtc { get; } 
}