# pvWay Logger Service for dotNet core 6

## Description
This nuget provides several very intuitive LoggerService implementations of the *PvWay.LoggerService.Abstractions.nc6* ILoggerService interface :
* ConsoleLogger
  * This colorful implementation uses Console.WriteLine outputting logs to the standard out.
* MuteLogger
  * As the name sounds this implementation can be used to injecting a silent logger. This can be handy for unit testing.
* Microsoft Logger
    * Uses the Microsoft.Extensions.Logging.Logger for outputting logs
* Ms Console Logger
    * Uses the Microsoft ConsoleLogger extension
* PersistenceLogger
    * Enables you to connect any persistence layer for storing logs into the storage of your choice
    * Have a look to 
      * the Microsoft SQL persistence layer *PvWay.LoggerService.MsSqlLogWriter.nc6* nuGet package
      * or the PostgreSQL persistence layer *PvWay.LoggerService.PgSqlLogWriter.nc6* nuGet package
* Multichannel Logger
    * Will output log to a collection of loggers
* UnitTest Logger
    * Allows you to perform asserts on logs content


## ILoggerService Methods

All methods include both a synchronous and an asynchronous signature.

See here after the main methods:

``` csharp
void Log(
    string message,
    SeverityEnum severity = SeverityEnum.Debug,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    string message,
    SeverityEnum severity = SeverityEnum.Debug,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);
        
void Log(
    IEnumerable<string> messages,
    SeverityEnum severity,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    IEnumerable<string> messages,
    SeverityEnum severity,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

void Log(
    Exception e,
    SeverityEnum severity = SeverityEnum.Fatal,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    Exception e,
    SeverityEnum severity = SeverityEnum.Fatal,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

void Log(
    IMethodResult result,
    string? topic,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    IMethodResult result,
    string? topic,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

```

## Usage

### Console Logger

#### Code
``` csharp
using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

public static class ConsoleLoggerDemo
{
    public static async Task FactorAndLogAsync()
    {
        var consoleLs = PvWayLoggerService.CreateConsoleLoggerService();

        var e = new Exception("Some exception");
        await consoleLs.LogAsync(e);

        await consoleLs.LogAsync("This is ok", SeverityEnum.Ok);
        await consoleLs.LogAsync("This is debug");
        await consoleLs.LogAsync("This is an info", SeverityEnum.Info);
        await consoleLs.LogAsync("This is a warning", SeverityEnum.Warning);
        await consoleLs.LogAsync("This is an error", SeverityEnum.Error);
        await consoleLs.LogAsync("This is a fatal", SeverityEnum.Fatal);
    }

    public static async Task InjectAndLogAsync()
    {
        var services = new ServiceCollection();

        // provisions the different loggerServices
        // ConsoleLogger, MuteLogger, MsConsoleLogger...
        services.AddPvWayLoggerServices(ServiceLifetime.Transient);

        var sp = services.BuildServiceProvider();

        // Retrieve the ConsoleLogger
        var consoleLs = sp.GetService<IPvWayConsoleLoggerService>()!;

        // Use it
        await consoleLs.LogAsync("Not that complex after all");
    }

}
```
#### Output

<p style="color: red;">
crit:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.14
    'Exception: Some exception
StackTrace: '
</p>

<p style="color: lightgreen">
8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.16
    'This is ok'
</p>

<p>
debg:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.17
    'This is debug'
</p>

<p style="color: darkcyan">
info:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.18
    'This is an info'
</p>


<p style="color: gold">
warn:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.19
    'This is a warning'
</p>

<p style="color: darkred">
fail:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.20
    'This is an error'
</p>

<p style="color: red;">
crit:8/27/2023 6:25:50 AM TDHPRO18A.FactorAndLogAsync.21
    'This is a fatal'
</p>

debg:8/27/2023 6:25:50 AM TDHPRO18A.InjectAndLogAsync.38
    'Not that complex after all'


### MsConsoleLogger
#### Code
``` csharp
using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

internal static class MsConsoleLoggerDemo
{
    public static async Task FactorAndLog()
    {
        var msConsoleLs = PvWayLoggerService.CreateMsConsoleLoggerService();
        var e = new Exception("Some exception");
        await msConsoleLs.LogAsync(e);

        await msConsoleLs.LogAsync("This is ok", SeverityEnum.Ok);
        await msConsoleLs.LogAsync("This is debug");
        await msConsoleLs.LogAsync("This is an info", SeverityEnum.Info);
        await msConsoleLs.LogAsync("This is a warning", SeverityEnum.Warning);
        await msConsoleLs.LogAsync("This is an error", SeverityEnum.Error);
        await msConsoleLs.LogAsync("This is a fatal", SeverityEnum.Fatal);
    }

    public static async Task InjectAndLog()
    {
        var services = new ServiceCollection();

        services.AddPvWayLoggerServices(ServiceLifetime.Transient);

        var sp = services.BuildServiceProvider();

        var msConsoleLs = sp.GetService<IPvWayMsConsoleLoggerService>()!;

        await msConsoleLs.LogAsync("This goes to the Microsoft Console Logger");
    }
}
```
#### Output

crit: PvWayMsConsoleLogger[0]
<br>
user:'(null)', companyId:'(null)', topic:'(null)', machineName:'TDHPRO18A', memberName:'FactorAndLog', filePath:'C:\GitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.LoggerServiceLab.nc6\MsConsoleLoggerDemo.cs', lineNumber:13, message: 'Exception: Some exception
StackTrace: ', date: 2023-08-27 06:25:50.504

info: PvWayMsConsoleLogger[0]
<br>
user:'(null)', companyId:'(null)', topic:'(null)', machineName:'TDHPRO18A', memberName:'FactorAndLog', filePath:'C:\GitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.LoggerServiceLab.nc6\MsConsoleLoggerDemo.cs', lineNumber:17, message: 'This is an info', date: 2023-08-27 06:25:50.512

warn: PvWayMsConsoleLogger[0]
<br>
user:'(null)', companyId:'(null)', topic:'(null)', machineName:'TDHPRO18A', memberName:'FactorAndLog', filePath:'C:\GitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.LoggerServiceLab.nc6\MsConsoleLoggerDemo.cs', lineNumber:18, message: 'This is a warning', date: 2023-08-27 06:25:50.512

fail: PvWayMsConsoleLogger[0]
<br>
user:'(null)', companyId:'(null)', topic:'(null)', machineName:'TDHPRO18A', memberName:'FactorAndLog', filePath:'C:\GitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.LoggerServiceLab.nc6\MsConsoleLoggerDemo.cs', lineNumber:19, message: 'This is an error', date: 2023-08-27 06:25:50.512

crit: PvWayMsConsoleLogger[0]
<br>
user:'(null)', companyId:'(null)', topic:'(null)', machineName:'TDHPRO18A', memberName:'FactorAndLog', filePath:'C:\GitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.LoggerServiceLab.nc6\MsConsoleLoggerDemo.cs', lineNumber:20, message: 'This is a fatal', date: 2023-08-27 06:25:50.512

// notice that due to the Ms Console config debug messages are filtered out

### UnitTestingLogger

``` csharp
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.Tests.nc6;

[TestFixture]
public class Tests
{

    [Test]
    public async Task Test1()
    {
        var ls = PvWayLoggerService.CreateUTestingLoggerService();

        // inject the unit testing logger to the class
        // so that it will be possible to retrieve the logs

        var svc = new SomeClassToTest(ls);

        await svc.PerformSomeActionAsync();

        Assert.Multiple(() =>
        {
            Assert.That(ls.LogRows, Is.Not.Empty);
            Assert.That(ls.LogRows.Count, Is.EqualTo(2));
            Assert.That(ls.HasLog("before writing"), Is.True);
        });

        var fRow = ls.FindFirstMatchingRow("writing");
        Assert.Multiple(() =>
        {
            Assert.That(fRow, Is.Not.Null);
            StringAssert.Contains("before", fRow?.Message);
        });

        var lRow = ls.FindLastMatchingRow("writing");
        Assert.Multiple(() =>
        {
            Assert.That(lRow, Is.Not.Null);
            StringAssert.Contains("after", lRow?.Message);
        });

    }
}

internal class SomeClassToTest
{
    private readonly ILoggerService _ls;

    public SomeClassToTest(
        ILoggerService ls)
    {
        _ls = ls;
    }

    public async Task PerformSomeActionAsync()
    {
        await _ls.LogAsync("before writing to the console");
        Console.WriteLine("Hello there");
        await _ls.LogAsync("after writing to the console");
    }
}
```

## See Also

* [pvWay.MsSqlLogWriter.nc6](https://www.nuget.org/packages/PvWay.LoggerService.MsSqlLogWriter.nc6) Implementation for Ms SQL Database


* [pvWay.PgSqlLogWriter.nc6](https://www.nuget.org/packages/PvWay.LoggerService.PgSqlLogWriter.nc6) Implementation for PostgreSQL Database


Take also a look to the [MethodResultWrapper](https://www.nuget.org/packages/pvWay.MethodResultWrapper.Core/) nuGet


