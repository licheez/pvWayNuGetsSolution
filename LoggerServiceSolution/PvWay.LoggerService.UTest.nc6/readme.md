# pvWay UTestLoggerService for dotNet Core 6

## Description
This nuget implements the ILoggerService by passing a UnitTest logWriter enabling you to perform Asserts on log content 

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

The **AddPvWayUTestLoggerService** method extends the IServiceCollection

The default lifetime is **Scoped** and the default minimum log level is **Trace**... i.e. logging everything

The method returns a IUTestLogWriter object. 

That object UTestLogWriter object will allow you to perform Asserts on log content during your unit tests.

``` csharp
    /// <summary>
    /// Injects a transient IUTestLoggerService
    /// </summary>
    /// <param name="services"></param>
    /// <returns>IUTestLogWriter</returns>
    public static IUTestLogWriter AddPvWayUTestLoggerService(
        this IServiceCollection services)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(SeverityEnu.Trace));
        
        var logWriter = new UTestLogWriter();
        services.TryAddTransient<IUTestLogWriter>(_ => logWriter);
        services.TryAddTransient<ILoggerService, UTestLoggerService>();
        services.TryAddTransient<IUTestLoggerService, UTestLoggerService>();
        
        return logWriter;
    }
```

## IUTestLogWriter interface

``` csharp
  public interface IUTestLogWriter : ILogWriter, IDisposable, IAsyncDisposable
  {
    IEnumerable<ILoggerServiceRow> LogRows { get; }

    bool HasLog(string term);

    ILoggerServiceRow? FindFirstMatchingRow(string term);

    ILoggerServiceRow? FindLastMatchingRow(string term);
  }
```

## Static factories

The **PvWayConsoleLogger** static class also exposes two public **Create** methods enabling to factor the service from your own code

``` csharp
    public static IUTestLogWriter CreateUTestLogWriter()
    {
        return new UTestLogWriter();
    }
    
    public static IUTestLoggerService CreateService(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }
    
    public static IUTestLoggerService CreateService(
        out IUTestLogWriter utLw)
    {
        utLw = CreateUTestLogWriter();
        return new UTestLoggerService(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }

    
    public static IUTestLoggerService<T> CreateService<T>(
        IUTestLogWriter utLw)
    {
        return new UTestLoggerService<T>(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }

    public static IUTestLoggerService<T> CreateService<T>(
        out IUTestLogWriter utLw)
    {
        utLw = CreateUTestLogWriter();
        return new UTestLoggerService<T>(
            new LoggerServiceConfig(SeverityEnu.Trace), 
            utLw);
    }
```


## Usage

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.UTest.nc8;

Console.WriteLine("Hello, UTestLoggerService");
Console.WriteLine();

var services = new ServiceCollection();
var lw = services.AddPvWayUTestLoggerService();
var sp = services.BuildServiceProvider();
var ls = sp.GetRequiredService<ILoggerService>();

ls.Log("This is a trace test log message", SeverityEnu.Trace);
ls.Log("This is a debug test log message");
ls.Log("This is an info test log message", SeverityEnu.Info);
ls.Log("This is a warning test log message", SeverityEnu.Warning);
ls.Log("This is an error test log message", SeverityEnu.Error);
ls.Log("This is a fatal test log message", SeverityEnu.Fatal);

ls.Log(LogLevel.Trace, "MsLog trace");

foreach (var row in lw.LogRows)
{
    Console.WriteLine(row.Message);
}
```

Happy coding