# Ms SQL Semaphore Service for .Net Core 6 by pvWay

This nuGet implements a tiny semaphore service based on a Ms SQL database.


### Pre-conditions for this service to work properly

The connection string provided should allow crudding rows
into/from a table that should conform to the following DDL.

### Log table definition example
``` sql

CREATE TABLE [app].[AppSemaphore]
(
	[Name]			VARCHAR(50)		NOT NULL,
	[Owner]			VARCHAR(128)	NOT NULL,
	[CreateDateUtc]	DATETIME		NOT NULL,
	[UpdateDateUtc]	DATETIME		NOT NULL

	CONSTRAINT [PK_app.AppSemaphore] 
	    PRIMARY KEY CLUSTERED ([Name] ASC)
);
    
```
### Columns
 
#### Name

The Name column is used for identifying the semaphore.
This is VARCHAR 50 column

#### Owner

The Owner column is used for identifying the owner of the metaphore.
It is usually populated with the Environment.MachineName value but you
can use any value.
This is VARCHAR 128 column


#### CreateDateUtc

This stores the UTC timestamp of the creation of the semaphore

#### UpdateDateUtc

This stores the UTC timestamp of the lastupdate of the semaphore

### Work principle

#### Acquiring the semaphore

The service tries to insert a row into the Semaphore table in the DB. 
The name of the semaphore is used for populating the "Name" column
in the table. The "Name" column is actually a primary key. As such
when the INSERT command executes, it fails if there is already one
row in the Db with the same semaphore name.

#### Releasing the semaphore
Releasing a semaphore is implemented as a simple DELETE for the corresponding
row in the Db.

#### Touching the semaphore
When acquiring a semaphore you need to pass the onwer name and a timeout
timespan.

The timeout value corresponds to the max life time allowed since 
the last touch for a given semaphore.

When another process tries to acquire the semaphore and the life time is
above the timeout the semaphore is automatically released so that a died process 
will not lock a semaphore forever.

### Usage

```csharp

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

```
Happy coding
