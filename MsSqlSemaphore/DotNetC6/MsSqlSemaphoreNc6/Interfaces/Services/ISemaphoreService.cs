using pvWay.MsSqlSemaphore.nc6.Interfaces.Enums;
using pvWay.MsSqlSemaphore.nc6.Interfaces.Model;

namespace pvWay.MsSqlSemaphore.nc6.Interfaces.Services;

public interface ISemaphoreService
{
    string SemaphoreName { get; }
    Task<DbSemaphoreStatusEnum> AcquireSemaphoreAsync(
        string owner, TimeSpan timeout);
    Task TouchSemaphoreAsync();
    Task ReleaseSemaphoreAsync();
    Task<IDbSemaphore?> GetSemaphoreAsync();
}