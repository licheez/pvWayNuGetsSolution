using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

public interface IPvWayLogRow
{
    int Id { get; }
    string? UserId { get; }
    string? CompanyId { get; }
    string? Topic { get; }
    SeverityEnum Severity { get; }
    string MachineName { get; }
    string MemberName { get; }
    string FilePath { get; }
    int LineNumber { get; }
    string Message { get; }
    DateTime CreationDateUtc { get; }
}