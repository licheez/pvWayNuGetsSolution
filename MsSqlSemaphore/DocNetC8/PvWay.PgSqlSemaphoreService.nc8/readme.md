# PgSql SemaphoreService for .Net Core 8 by pvWay

This nuGet implements the SemaphoreService interfaces using a tiny DAO connection towards an Postgres Database.

## Interfaces and enums as defined in the abstraction nuGet

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

Small object that holds some useful information about the semaphore 

```csharp
namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface ISemaphoreInfo
{
    SemaphoreStatusEnu Status { get; }
    
    string Name {get; }
    string Owner { get; }
    TimeSpan Timeout { get; }
    DateTime ExpiresAtUtc { get; }
    DateTime CreateDateUtc { get; }
    DateTime UpdateUtcDate { get; }
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
    Task<ISemaphoreInfo> AcquireSemaphoreAsync(
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
    
    /// <summary>
    /// this method provides a mutex (semaphore) protected
    /// work session when a provided work can be performed.
    /// </summary>
    /// <param name="semaphoreName">The name of the mutex</param>
    /// <param name="owner">the owner of the mutex (usually the machineName)</param>
    /// <param name="timeout">The validity time of the lock</param>
    /// <param name="workAsync">
    /// The work to be performed.
    /// </param>
    /// <param name="notify">An optional notifier that will be used
    /// for notifying sleep times</param>
    /// <param name="sleepBetweenAttemptsInSeconds">
    /// The number of seconds between two attempts for locking the semaphore</param>
    /// <typeparam name="T">Type return by the workAsync method</typeparam>
    /// <returns>The T result of the function invoked</returns>
    Task<T> IsolateWorkAsync<T>(
        string semaphoreName, string owner,
        TimeSpan timeout,
        Func<Task<T>> workAsync,
        Action<string>? notify = null,
        int sleepBetweenAttemptsInSeconds = 15);
}
```

## Injection & Factories

The AddPvWayPgSqlSemaphoreService request several parameters

* schema name and table name
* getCsAsync: an async method for retrieving the connection string of the database
  * make sure to pass a db owner connection string if the semaphore table is not yet present into the db
* logException: a delegate that will log exceptions
* logInfo: an optional delegate for getting verbose notifications

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.PgSqlSemaphoreService.nc8.Services;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.PgSqlSemaphoreService.nc8.Di;

public static class PvWayPgSqlSemaphoreService
{
    public static ISemaphoreService Create(
        string schemaName,
        string tableName,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException,
        Action<string>? logInfo)
    {
        var config = new SemaphoreServiceConfig(
            schemaName, tableName, getCsAsync,
            logException, logInfo);
        return new Services.SemaphoreService(config);
    }
    
    public static void AddPvWayMsSqlSemaphoreService(
        this IServiceCollection services,
        string schemaName,
        string tableName,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException,
        Action<string>? logInfo)
    {
        services.TryAddSingleton<ISemaphoreServiceConfig>(_ =>
            new SemaphoreServiceConfig(
                schemaName, tableName, 
                getCsAsync, 
                logException, logInfo));
        services.AddTransient<
            ISemaphoreService, 
            Services.SemaphoreService>();
    }
}
```

## Usage

``` csharp
using Microsoft.Extensions.DependencyInjection;
using PvWay.PgSqlSemaphoreService.nc8.Di;
using PvWay.SemaphoreService.Abstractions.nc8;

Console.WriteLine("Hello, PvWay.pgSqlSemaphoreService");

var services = new ServiceCollection();

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=postgres;" +
                       "Password=S0mePwd_;";

services.AddPvWayMsSqlSemaphoreService(
    "mySchema",
    "mySemaphoreTable",
    async () => await Task.FromResult(pgSqlCs),
    Console.WriteLine,
    Console.WriteLine);
    
var sp = services.BuildServiceProvider();

var svc = sp.GetRequiredService<ISemaphoreService>();

const string resourceName = "SharedResource";

// Acquire the semaphore
Console.WriteLine("Program: acquiring the semaphore");
var si = await svc.AcquireSemaphoreAsync(resourceName,
	Environment.MachineName, TimeSpan.FromSeconds(1));
Console.WriteLine($"Program: semaphore {resourceName} was acquired by {si.Owner}");
Console.WriteLine();

// Try acquiring a second time
// the semaphore was created with a timeout of 1 second
// as such acquiring the semaphore should not succeed
// because the semaphore is still in use
Console.WriteLine("Program: acquiring again the semaphore");
si = await svc.AcquireSemaphoreAsync(resourceName,
	Environment.MachineName, TimeSpan.FromSeconds(1));
Console.WriteLine($"Program: status={si.Status}");
Console.WriteLine();

//
// // extend the life time of the semaphore by touching its
// // last update date
Console.WriteLine("Program: extend the semaphore");
await svc.TouchSemaphoreAsync(resourceName);
Console.WriteLine();

// let's display the semaphore values
var semaphore = await svc.GetSemaphoreAsync(resourceName);
Console.WriteLine(semaphore);
Console.WriteLine();

// // sleep 2 second so that the semaphore expires
Console.WriteLine("Program: now sleeping 2 seconds");
Thread.Sleep(TimeSpan.FromSeconds(2));
Console.WriteLine();

// // Try acquiring a third time
// // the semaphore expired 
// // as such the acquiring method will force release it
// // warning: the semaphore is not acquired at this stage
Console.WriteLine("Program: trying to acquire a third time");
await svc.AcquireSemaphoreAsync(resourceName,
Environment.MachineName, TimeSpan.FromSeconds(1));
Console.WriteLine();

// // acquiring the released semaphore
// // the semaphore was forced release at the previous step
// // as such acquiring it again will succeed
Console.WriteLine("Program: trying to acquire again");
await svc.AcquireSemaphoreAsync(resourceName,
Environment.MachineName, TimeSpan.FromSeconds(1));
Console.WriteLine();

// finally release the semaphore
Console.WriteLine("Program: trying to acquire again");
await svc.ReleaseSemaphoreAsync(resourceName);
Console.WriteLine();

// Let's now test the isolate work method
var res = await svc.IsolateWorkAsync(
	resourceName, Environment.MachineName,
	TimeSpan.FromSeconds(10),
	() => Task.FromResult("some work was done alone"),
	Console.WriteLine,
	5);
Console.WriteLine(res);
```


Happy coding
