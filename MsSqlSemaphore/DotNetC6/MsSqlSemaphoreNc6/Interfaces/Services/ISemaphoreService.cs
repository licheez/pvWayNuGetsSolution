using pvWay.MsSqlSemaphore.nc6.Interfaces.Enums;
using pvWay.MsSqlSemaphore.nc6.Interfaces.Model;

namespace pvWay.MsSqlSemaphore.nc6.Interfaces.Services;

public interface ISemaphoreService
{
    Task<DbSemaphoreStatusEnum> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout);
    Task TouchSemaphoreAsync(string name);
    Task ReleaseSemaphoreAsync(string name);
    Task<IDbSemaphore?> GetSemaphoreAsync(string name);
}