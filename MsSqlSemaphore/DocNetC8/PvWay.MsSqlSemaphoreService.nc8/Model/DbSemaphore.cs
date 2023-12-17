using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.MsSqlSemaphoreService.nc8.Model;

internal class DbSemaphore(
    SemaphoreStatusEnu status,
    string? owner,
    TimeSpan timeout,
    DateTime createDateUtc,
    DateTime updateUtcDate)
    : ISemaphoreInfo
{
    public SemaphoreStatusEnu Status { get; } = status;
    public string Owner { get; } = owner??"unknown owner";
    public TimeSpan Timeout { get; } = timeout;
    public DateTime ExpiresAtUtc => UpdateUtcDate.Add(Timeout);
    public DateTime CreateDateUtc { get; } = createDateUtc;
    public DateTime UpdateUtcDate { get; } = updateUtcDate;

    public DbSemaphore(
        SemaphoreStatusEnu status,
        ISemaphoreInfo si) : this(
        status, si.Owner,
        si.Timeout,
        si.CreateDateUtc, si.UpdateUtcDate)
    {
    }
}
