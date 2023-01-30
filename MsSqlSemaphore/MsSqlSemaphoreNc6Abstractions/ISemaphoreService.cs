namespace pvWay.MsSqlSemaphore.nc6.abstractions;

public interface ISemaphoreService
{
    Task<DbSemaphoreStatusEnum> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout);
    Task TouchSemaphoreAsync(string name);
    Task ReleaseSemaphoreAsync(string name);
    Task<IDbSemaphore?> GetSemaphoreAsync(string name);
}