// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PvWay.MsSqlSemaphoreService.nc8.Di;
using PvWay.SemaphoreService.Abstractions.nc8;

Console.WriteLine("Hello, PvWay Semaphore Service");
Console.WriteLine("==============================");

// pre-conditions
// the database contains the semaphore table

/*
Pre-Conditions
==============

The database must contain the semaphore table
 
The table structure must respect the following DDL.

The the schema and the name can be modified.

CREATE TABLE [dbo].[Semaphore]
(
	[Name]				VARCHAR(50)		NOT NULL,
	[Owner]				VARCHAR(128)	NOT NULL,
	[TimeoutInSeconds]	INT				NOT NULL,
	[CreateDateUtc]		DATETIME		NOT NULL,
	[UpdateDateUtc]		DATETIME		NOT NULL

	CONSTRAINT [PK_app.AppSemaphore] 
	    PRIMARY KEY CLUSTERED ([Name] ASC)
)
*/

// we need to read the app settings
var config = new ConfigurationBuilder()
	.AddJsonFile("appSettings.json", false, false)
	.Build();

// let's now provision the Semaphore service
var semaphoreSection = config.GetSection("pvWaySemaphoreService");

var services = new ServiceCollection();
services.AddPvWayMsSqlSemaphoreService(
	semaphoreSection,
	// a callback that gets the connection string
	() =>
	{
		var cs = config.GetConnectionString("semaphore")!;
		return Task.FromResult(cs);
	},
	// a callback that log exceptions
	Console.WriteLine, 
	// a callback that log some info
	Console.WriteLine);

var sp = services.BuildServiceProvider();

var svc = sp.GetService<ISemaphoreService>()!;

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
