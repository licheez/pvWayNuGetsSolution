# SemaphoreService abstractions for .Net Core 8 by pvWay

This nuGet brings the abstraction interfaces for the SemaphoreService

## Interfaces and enums

### SemaphoreStatus enum

This enum enumerates the different possible statuses of a semaphore when trying to acquire it

* **Acquired**: (success status) the semaphore was acquired
* **ReleasedInTheMeanTime**: the semaphore was locked by someone else but when getting more info it finally appeared released.
* **OwnedBySomeoneElse**: another process currently owns the semaphore. Other processes will have to wait until the semaphore will be released by the owner process.
* **ForcedReleased**: the semaphore was locked by another process that seems not being responding for a while. As such, the release of the semaphore was forced.

```csharp
namespace PvWay.SemaphoreService.Abstractions.nc8;

public enum SemaphoreStatusEnu
{
    Acquired,
    ReleasedInTheMeanTime,
    OwnedSomeoneElse,
    ForcedReleased
}
```

### IDbSemaphore

Small object that holds the name of the semaphore owner as well as the UTC date and time the owner process was touching(refreshing) the lock. 

```csharp
namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface ISemaphoreInfo
{
    string Owner { get; }
    DateTime LastTouchUtcDate { get; }
}
```

### ISemaphoreService
```csharp
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
```

Happy coding
