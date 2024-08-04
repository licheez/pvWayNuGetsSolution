# pvWay HybridLoggerService for dotNet Core 6

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

The **AddPvWayHybridLoggerService** method extends the IServiceCollection

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.Hybrid.nc6;

public static class PvWayHybridLogger
{
    // CREATORS
    public static IHybridLoggerService CreateService(
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        params ILogWriter[] logWriters)
    {
        var sConfig = new LoggerServiceConfig(minLogLevel);
        var hConfig = new HybridLoggerConfig(logWriters);
        return new HybridLoggerService(sConfig, hConfig);
    }
    
    // FACTORY
    public static void AddPvWayHybridLoggerServiceFactory(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        params ILogWriter[] logWriters)
    {
        services.AddSingleton<ILoggerServiceFactory<IHybridLoggerService>>(_ => 
            new HybridLoggerServiceFactory(minLogLevel, logWriters));
    }
    
    // SERVICE
    public static void AddPvWayHybridLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        params ILogWriter[] logWriters)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddSingleton<HybridLoggerConfig>(_ =>
            new HybridLoggerConfig(logWriters));
        
        RegisterService(services, lifetime);
    }
    
    /// <summary>
    /// This will search the service container for a ConsoleLogWriter
    /// and a SqlLogWriter for creating a HybridLoggerConfig
    /// </summary>
    /// <param name="services"></param>
    /// <param name="minLogLevel"></param>
    /// <param name="lifetime"></param>
    public static void AddPvWayHybridLoggerService(
        this IServiceCollection services,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<HybridLoggerConfig>(sp =>
        {
            var cLw = sp.GetRequiredService<IConsoleLogWriter>();
            var sLw = sp.GetRequiredService<ISqlLogWriter>();
            return new HybridLoggerConfig(cLw, sLw);
        }); 
        
        RegisterService(services, lifetime);
    }
    
    
    private static void RegisterService(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        var descriptors = new List<ServiceDescriptor>
        {
            new(typeof(IHybridLoggerService),
                typeof(HybridLoggerService),
                lifetime),
            new(typeof(IHybridLoggerService<>),
                typeof(HybridLoggerService<>),
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
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.Hybrid.nc6;
using PvWay.LoggerService.PgSql.nc6;
using PvWay.LoggerService.SeriConsole.nc6;

// add a consoleLogWriter (here SeriConsole)
var services = new ServiceCollection();
services.AddPvWaySeriConsoleLogWriter();

// add a sql logWriter (here PgSql)
const string pCs = "Server=localhost;" +
                   "Database=postgres;" +
                   "User Id=postgres;" +
                   "Password=S0mePwd_;";
services.AddPvWayPgSqlLogWriter(
    _ => Task.FromResult<string>(pCs));

// Inject an hybrid logger service that will
// combine the ConsoleLogWriter and the SqlLogWriter
services.AddPvWayHybridLoggerService();

var sp = services.BuildServiceProvider();

var hLoggerService = sp.GetRequiredService<IHybridLoggerService>();

// the next method call will output on both the console and the database
await hLoggerService.LogAsync("Hello Hybrid Logger", SeverityEnu.Info);
```

Happy coding