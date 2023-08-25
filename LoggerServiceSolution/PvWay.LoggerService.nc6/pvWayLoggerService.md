# pvWay Logger Service for dotNet core 6

## Description
This nuget provides several very intuitive LoggerService implementations of the *PvWay.LoggerService.Abstractions.nc6* ILoggerService interface :
* ConsoleLogger
* MuteLogger
* Microsoft Logger
* PersistenceLogger
* Ms Console Logger
* Multichannel Logger

### ConsoleLogger
* This colorful implementation uses Console.WriteLine outputting logs to the standard out.

### Mutelogger
* As the name sounds this implementation can be used to injecting a silent logger. This can be handy for unit testing.

### Microsoft Logger
* Uses the Microsoft.Extensions.Logging.Logger for outputting logs

### Persistent Logger
* Enables you to connect any persistence layer for storing logs into the storage of your choice
* Have a look to 
  * the Microsoft SQL persistence layer *PvWay.LoggerService.MsSqlLogWriter.nc6* nuGet package
  * or the PostgreSQL persistence layer *PvWay.LoggerService.PgSqlLogWriter.nc6* nuGet package

## Methods

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

``` csharp
using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.nc6;

public class ConsoleLoggerDemo
{
    public async Task<double> HowToUseTheConsoleLogger(
        double x)
    {
        Console.WriteLine("Hello, ConsoleLoggerService");
        Console.WriteLine("---------------------------");
        Console.WriteLine();

        var consoleLs = PvWayLoggerService.CreateConsoleLoggerService();

        try
        {
            // dividing by zero throws an exception
            return x / 0;
        }
        catch (Exception e)
        {
            await consoleLs.LogAsync(e);
            throw;
        }
    }

    public async Task AndWithDependencyInjection()
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

## See Also

* [pvWay.MsSqlLogWriter.nc6](https://www.nuget.org/packages/PvWay.LoggerService.MsSqlLogWriter.nc6) Implementation for Ms SQL Database


* [pvWay.PgSqlLogWriter.nc6](https://www.nuget.org/packages/PvWay.LoggerService.PgSqlLogWriter.nc6) Implementation for PostgreSQL Database


Take also a look to the [MethodResultWrapper](https://www.nuget.org/packages/pvWay.MethodResultWrapper.Core/) nuGet


