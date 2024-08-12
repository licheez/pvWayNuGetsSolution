# pvWay SeriLoggerService for dotNet Core 8

## Description
This nuget implements the ILoggerService as a simple stdout console using the well known Serilog(tm) console skin nuGet

## Severity enum

``` csharp
public enum SeverityEnu
{
    Ok,         // "O"
    Trace,      // "T"
    Debug,      // "D"
    Info,       // "I"
    Warning,    // "W"
    Error,      // "E"
    Fatal       // "F"
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

The **PvWaySerilogConsoleLogger** method extends the IServiceCollection

The default lifetime is **Singleton** and the default minimum log level is **Trace**... i.e. logging everything

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

public static class PvWaySerilogConsoleLogger
{
    // LOGGER PROVIDER
    public static ILoggerProvider GetProvider(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleLoggerProvider(minLogLevel);
    }
    
    // CREATE
    public static IConsoleLogWriter CreateWriter()
    {
        return new SerilogConsoleWriter();
    }
    
    public static ISeriConsoleLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService(
            new LoggerServiceConfig(minLogLevel),
            new SerilogConsoleWriter());
    }

    public static ISeriConsoleLoggerService<T> CreateService<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new SerilogConsoleService<T>(
            new LoggerServiceConfig(minLogLevel),
            new SerilogConsoleWriter());
    }
    
    // LOG WRITER
    public static void AddPvWaySeriConsoleLogWriter(
        this IServiceCollection services)
    {
        services.TryAddSingleton<
            IConsoleLogWriter, SerilogConsoleWriter>();
    }
    
    // FACTORY
    public static void AddPvWaySeriConsoleLoggerFactory(
        this IServiceCollection services)
    {
        services.TryAddSingleton<
            ILoggerServiceFactory<IConsoleLoggerService>,
            SerilogConsoleFactory>();
    }
    
    // SERVICES
    public static void AddPvWaySeriConsoleLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddPvWaySeriConsoleLogWriter();
        
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(ISeriConsoleLoggerService),
                typeof(SerilogConsoleService),
                lifetime),
            new(typeof(IConsoleLoggerService),
                typeof(SerilogConsoleService),
                lifetime),
            new(typeof(ISeriConsoleLoggerService<>),
                typeof(SerilogConsoleService<>),
                lifetime),
            new(typeof(IConsoleLoggerService<>),
                typeof(SerilogConsoleService<>),
                lifetime),
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
using PvWay.LoggerService.SeriConsole.nc8;

Console.WriteLine("Hello, SeriConsoleLoggerService");
Console.WriteLine();

var services = new ServiceCollection();
services.AddPvWaySeriConsoleLoggerService();
var sp = services.BuildServiceProvider();
var ls = sp.GetRequiredService<ILoggerService>();

ls.Log("This is a trace test log message", SeverityEnu.Trace);
ls.Log("This is a debug test log message");
ls.Log("This is an info test log message", SeverityEnu.Info);
ls.Log("This is a warning test log message", SeverityEnu.Warning);
ls.Log("This is an error test log message", SeverityEnu.Error);
ls.Log("This is a fatal test log message", SeverityEnu.Fatal);

ls.Log(LogLevel.Trace, "MsLog trace");
```

Happy coding