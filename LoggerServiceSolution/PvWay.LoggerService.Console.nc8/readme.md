# pvWay ConsoleLoggerService for dotNet Core 8

## Description
This nuget implements the ILoggerService as a simple stdout console using Console.WriteLine statements. 

The output is colored depending on the severity. 

## Severity enum

``` csharp
public enum SeverityEnu
{
    Ok,         // "O" Green
    Trace,      // "T" Gray
    Debug,      // "D" White
    Info,       // "I" DarkCyan
    Warning,    // "W" DarkYellow
    Error,      // "E" DarkRed
    Fatal       // "F" Red
}
```

## Methods

All methods include both a synchronous and an asynchronous signature.

The class also implement the ILogger interface (microsoft.logging)

See here after the main methods:

``` csharp
void Log(
    string message,
    SeverityEnu severity = SeverityEnu.Debug,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    string message,
    SeverityEnu severity = SeverityEnu.Debug,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);
        
void Log(
    IEnumerable<string> messages,
    SeverityEnu severity,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    IEnumerable<string> messages,
    SeverityEnu severity,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

void Log(
    Exception e,
    SeverityEnu severity = SeverityEnu.Fatal,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    Exception e,
    SeverityEnu severity = SeverityEnu.Fatal,
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

## Injection

The **AddPvWayConsoleLoggerService** method extends the IServiceCollection

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Console.nc6;

public static class PvWayConsoleLogger
{
    // LOGGER PROVIDER
    public static ILoggerProvider GetProvider(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerProvider(minLogLevel);
    }
    
    // CREATORS
    public static IConsoleLogWriter CreateWriter()
    {
        return new ConsoleLogWriter();
    }

    public static IConsoleLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService(
            new LoggerServiceConfig(minLogLevel),
            new ConsoleLogWriter());
    }

    public static IConsoleLoggerService<T> CreateService<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new ConsoleLoggerService<T>(
            new LoggerServiceConfig(minLogLevel),
            new ConsoleLogWriter());
    }

   // WRITER 
    public static void AddPvWayConsoleLogWriter(
        this IServiceCollection services)
    {
        services.TryAddSingleton<IConsoleLogWriter, ConsoleLogWriter>();
    }
   
    // FACTORY
    public static void AddPvWayConsoleLoggerFactory(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            ConsoleLoggerServiceFactory>();
    }
    
    // SERVICE
    public static void AddPvWayConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayConsoleLogWriter();

        const ServiceLifetime lifetime = ServiceLifetime.Singleton;
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(IConsoleLoggerService),
                typeof(ConsoleLoggerService),
                lifetime),
            new(typeof(IConsoleLoggerService<>),
                typeof(ConsoleLoggerService<>),
                lifetime)
        };
        foreach (var sd in descriptors)
        {
            services.TryAdd(sd);
        }
    }   
}
```


## Usage

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.Console.nc8;

Console.WriteLine("Hello, ConsoleLoggerService");
Console.WriteLine();

var services = new ServiceCollection();
services.AddPvWayConsoleLoggerService();
var sp = services.BuildServiceProvider();
var ls = sp.GetRequiredService<IConsoleLoggerService>();

ls.Log("This is a trace test log message", SeverityEnu.Trace);
ls.Log("This is a debug test log message");
ls.Log("This is an info test log message", SeverityEnu.Info);
ls.Log("This is a warning test log message", SeverityEnu.Warning);
ls.Log("This is an error test log message", SeverityEnu.Error);
ls.Log("This is a fatal test log message", SeverityEnu.Fatal);

ls.Log(LogLevel.Trace, "MsLog trace");
```

Happy coding