using System.Globalization;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.MsSqlSemaphoreService.nc8.Model;

internal class DbSemaphore(
    SemaphoreStatusEnu status,
    string name,
    string? owner,
    TimeSpan timeout,
    DateTime createDateUtc,
    DateTime updateDateUtc)
    : ISemaphoreInfo
{
    public SemaphoreStatusEnu Status { get; } = status;
    public string Name { get; } = name;
    public string Owner { get; } = owner??"unknown owner";
    public TimeSpan Timeout { get; } = timeout;
    public DateTime ExpiresAtUtc => UpdateDateUtc.Add(Timeout);
    public DateTime CreateDateUtc { get; } = createDateUtc;
    public DateTime UpdateDateUtc { get; } = updateDateUtc;

    public override string ToString()
    {
        var createDt = CreateDateUtc.ToString(CultureInfo.InvariantCulture);
        var updateDt = UpdateDateUtc.ToString(CultureInfo.InvariantCulture);
        var expireDt = ExpiresAtUtc.ToString(CultureInfo.InvariantCulture);
        return $"semaphore {Name} owned by {Owner} " +
               $"created at {createDt} UTC, " +
               $"with a timeout of {Timeout} " +
               $"has been updated the last time at {updateDt} UTC " +
               $"and is due to expire at {expireDt} UTC";
    }

    public DbSemaphore(
        SemaphoreStatusEnu status,
        ISemaphoreInfo si) : this(
        status, si.Name, si.Owner,
        si.Timeout,
        si.CreateDateUtc, si.UpdateDateUtc)
    {
    }
}
