# pvWay SeriLoggerService for dotNet Core 6

## Description
This nuget implements the ILoggerService as a simple stdout console using the well-known Serilog(tm) console sink nuGet. It provides a clean and colorful console output with structured logging capabilities.

## Features
- **Serilog-based console output** with color-coded severity levels
- **Dependency Injection support** through IServiceCollection extensions
- **Microsoft.Extensions.Logging integration** (implements ILogger and ILoggerProvider)
- **IOptions pattern support** for configuration
- **Flexible severity level filtering** with string-based synonyms
- **Structured logging** with contextual information (machine name, caller info, user/company ID, topics)

## Severity enum

``` csharp
public enum SeverityEnu
{
    Ok,         // "O" - Not logged to console
    Trace,      // "T" - Verbose level
    Debug,      // "D" - Debug level  
    Info,       // "I" - Information level
    Warning,    // "W" - Warning level
    Error,      // "E" - Error level
    Fatal       // "F" - Fatal/Critical level
}
```

## Configuration

The service can be configured using the IOptions pattern through `appsettings.json`:

``` json
{
  "PvWayLoggerServiceConfig": {
    "MinLogLevel": 2,
    "MinLogLevelText": "info"
  }
}
```

The `MinLogLevelText` property supports various synonyms for convenience:
- **Trace**: "trace", "t", "verbose", "v"
- **Debug**: "debug", "d"
- **Info**: "info", "information", "i"
- **Warning**: "warning", "w"
- **Error**: "error", "e"
- **Fatal**: "fatal", "f", "critic", "critical", "c"

## Methods

All methods include both synchronous and asynchronous signatures.

The service implements both `ILogger` (Microsoft.Extensions.Logging) and `ILoggerService` (PvWay) interfaces.

### Context Methods

``` csharp
// Set user context for subsequent log calls
void SetUser(string? userId, string? companyId = null);

// Set topic for grouping related logs
void SetTopic(string? topic);
```

### Main Logging Methods

``` csharp
// Log a simple message
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

// Log multiple messages
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

// Log exceptions
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

// Log method results
void Log(
    IMethodResult result,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

Task LogAsync(
    IMethodResult result,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

// Log with explicit topic
void Log(
    string message,
    string? topic,
    SeverityEnu severity = SeverityEnu.Debug,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

void Log(
    IMethodResult result,
    string? topic,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = -1);

```

## Dependency Injection

The service provides extension methods for `IServiceCollection` to configure and register the logger using the **IOptions pattern**.

### Quick Start with IConfiguration

``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.SeriConsole.nc6;

// Setup configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

// Register the writer only
services.TryAddPvWayLoggerServiceSeriWriter(configuration);

// Or register the complete service with ILoggerProvider
services.TryAddPvWayLoggerServiceSeriService(configuration);

var sp = services.BuildServiceProvider();
var logger = sp.GetRequiredService<ILoggerService>();
```

### Extension Methods

``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.SeriConsole.nc6;

public static class PvWayLoggerServiceSeriConsoleDi
{
    // Register the Serilog console writer
    public static IServiceCollection TryAddPvWayLoggerServiceSeriWriter(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddLoggerServiceConfig(config);
        services.TryAddSingleton<ILogWriter, SerilogConsoleWriter>();
        services.TryAddSingleton<IConsoleLogWriter, SerilogConsoleWriter>();
        
        return services;
    }

    // Register the complete service including ILoggerProvider
    public static IServiceCollection TryAddPvWayLoggerServiceSeriService(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.TryAddPvWayLoggerServiceSeriWriter(config);
        
        services.TryAddSingleton<ILoggerService, SerilogConsoleService>();
        services.TryAddSingleton<IConsoleLoggerService, SerilogConsoleService>();
        services.TryAddSingleton<ISeriConsoleLoggerService, SerilogConsoleService>();
        
        services.TryAddSingleton<ILoggerProvider, SerilogConsoleLoggerProvider>();
        services.TryAddSingleton<IConsoleLoggerProvider, SerilogConsoleLoggerProvider>();
        
        return services;
    }
}
```


## Usage

### Basic Example

``` csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.SeriConsole.nc6;

Console.WriteLine("Hello, SeriConsoleLoggerService");
Console.WriteLine();

// Setup configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.TryAddPvWayLoggerServiceSeriService(configuration);

var sp = services.BuildServiceProvider();
var ls = sp.GetRequiredService<ILoggerService>();

// Log with different severity levels
ls.Log("This is a trace test log message", SeverityEnu.Trace);
ls.Log("This is a debug test log message", SeverityEnu.Debug);
ls.Log("This is an info test log message", SeverityEnu.Info);
ls.Log("This is a warning test log message", SeverityEnu.Warning);
ls.Log("This is an error test log message", SeverityEnu.Error);
ls.Log("This is a fatal test log message", SeverityEnu.Fatal);

// Works with Microsoft.Extensions.Logging.LogLevel too
ls.Log(LogLevel.Trace, "MsLog trace");

// Set user context
ls.SetUser("user123", "company456");
ls.Log("User-specific log message");

// Set topic for grouping
ls.SetTopic("OrderProcessing");
ls.Log("Processing order", SeverityEnu.Info);
```

### With appsettings.json

``` json
{
  "PvWayLoggerServiceConfig": {
    "MinLogLevelText": "info"
  }
}
```

Happy coding!
