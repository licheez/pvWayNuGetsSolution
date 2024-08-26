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
    Task IsolateWorkAsync(
        string semaphoreName, string owner,
        TimeSpan timeout,
        Func<Task> workAsync,
        Action<string>? notify = null,
        int sleepBetweenAttemptsInSeconds = 15);
}