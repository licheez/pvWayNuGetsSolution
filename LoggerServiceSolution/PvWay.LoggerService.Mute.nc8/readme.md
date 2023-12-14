# pvWay ConsoleLoggerService for dotNet Core 8

## Description
This nuget implements the ILoggerService as a silent logger. 

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

The **AddPvWayMuteLoggerService** method extends the IServiceCollection

The default lifetime is **Scoped** and the default minimum log level is **Trace**... i.e. logging everything

``` csharp
    public static void AddPvWayMuteLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        var sd = new ServiceDescriptor(
            typeof(ILoggerService), 
            typeof(MuteLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IMuteLoggerService), 
            typeof(MuteLoggerService),
            lifetime);
        services.Add(sd2);
    }
```

## Static factories

The **PvWayConsoleLogger** static class also exposes two public **Create** methods enabling to factor the service from your own code

``` csharp
    public static IMuteLoggerService Create(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService(new LoggerServiceConfig(minLogLevel));
    }

    public static IMuteLoggerService<T> Create<T>(
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        return new MuteLoggerService<T>(new LoggerServiceConfig(minLogLevel));
    }
```


## Usage

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.Mute.nc8;

Console.WriteLine("Hello, ConsoleLoggerService");
Console.WriteLine();

var services = new ServiceCollection();
services.AddPvWayMuteLoggerService();
var sp = services.BuildServiceProvider();
var ls = sp.GetService<ILoggerService>()!;

ls.Log("This is a trace test log message", SeverityEnu.Trace);
ls.Log("This is a debug test log message");
ls.Log("This is an info test log message", SeverityEnu.Info);
ls.Log("This is a warning test log message", SeverityEnu.Warning);
ls.Log("This is an error test log message", SeverityEnu.Error);
ls.Log("This is a fatal test log message", SeverityEnu.Fatal);

ls.Log(LogLevel.Trace, "MsLog trace");
```

Happy coding