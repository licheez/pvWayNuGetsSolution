using pvWay.MsSqlSemaphore.nc6.Services;

Console.WriteLine("Hello, Db Semaphore!");

const string connectionString =
    "Data Source=localhost;Initial Catalog=iota800_dev;integrated security=True;MultipleActiveResultSets=True;";

var dbSemaphoreService = new SemaphoreService(
    connectionString,
    "app", "AppSemaphore",
    "MySemaphore",
    Console.WriteLine,
    Console.WriteLine);

// let's release any existing Semaphore before beginning
await dbSemaphoreService.ReleaseSemaphoreAsync();

// Acquire the semaphore
// the semaphore was release in the above line
// as such acquiring the semaphore should succeed
await dbSemaphoreService.AcquireSemaphoreAsync(
    Environment.MachineName, TimeSpan.FromSeconds(1));

// Try acquiring a second time
// the semaphore was created with a timeout of 1 second
// as such acquiring the semaphore should not succeed
// because the semaphore is still in use
await dbSemaphoreService.AcquireSemaphoreAsync(
    Environment.MachineName, TimeSpan.FromSeconds(1));

// extend the life time of the semaphore by touching its
// last update date
await dbSemaphoreService.TouchSemaphoreAsync();

// let's display the semaphore values
var semaphore = await dbSemaphoreService.GetSemaphoreAsync();
Console.WriteLine($"semaphore {semaphore!.Owner} last updated {semaphore!.LastTouchUtcDate}");

// sleep 2 second so that the semaphore expires
Thread.Sleep(TimeSpan.FromSeconds(2));

// Try acquiring a third time
// the semaphore expired 
// as such the acquiring method will force release it
// warning: the semaphore is not acquired at this stage
await dbSemaphoreService.AcquireSemaphoreAsync(
    Environment.MachineName, TimeSpan.FromSeconds(1));

// acquiring the released semaphore
// the semaphore was forced release at the previous step
// as such acquiring it again will succeed
await dbSemaphoreService.AcquireSemaphoreAsync(
    Environment.MachineName, TimeSpan.FromSeconds(1));

// finally release the semaphore
await dbSemaphoreService.ReleaseSemaphoreAsync();
