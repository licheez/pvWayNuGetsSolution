namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface ISemaphoreService
{
    /// <summary>
    /// This method will actually try to acquire the semaphore
    /// </summary>
    /// <param name="name">The semaphore name</param>
    /// <param name="owner">
    /// The name of the process that tries to acquire the semaphore
    /// </param>
    /// <param name="timeout">
    /// The estimated time out timespan that the lock will stay active (if not refreshed).
    /// If the semaphore is locked longer than the timeout period it will be forced release
    /// by any other process trying to acquire the semaphore</param>
    /// <returns>On success the status will be Acquired.</returns>
    Task<SemaphoreStatusEnu> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout);
    
    /// <summary>
    /// Extend the validity timespan of the semaphore
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task TouchSemaphoreAsync(string name);
    
    /// <summary>
    /// Free the semaphore so that any other process can now acquire it
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task ReleaseSemaphoreAsync(string name);
    
    /// <summary>
    /// Get some info about a given semaphore
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<ISemaphoreInfo?> GetSemaphoreAsync(string name);
}