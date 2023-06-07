# pvWay LoggerService abstractions for dotNet Core 6

## Description
This nuget provides the interfaces and enums definitions for the PvWayLoggerService. This will enable you to abstract the LoggerService (and the LogWriter)

## Severity enum

For the sake of simplicity not all Microsoft LogLevels are implemented.

``` csharp
public enum SeverityEnum
{
    Ok,         // "O"
    Debug,      // "D"
    Info,       // "I"
    Warning,    // "W"
    Error,      // "E"
    Fatal,      // "F"
}
```

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

```

## Usage

``` csharp
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

// ConsoleLogger is an implementation of 
// the logger service that output colorful
// messages to the standard out
var ls = new ConsoleLogger();

// sync logging a simple debug message to the console
// --------------------------------------------------
ls.Log("simple debug message);

// async logging a warning message to the console
// ----------------------------------------------
async ls.LogAsync("this is a warning", SeverityEnum.Warning);

// logging an exception to the console
// -----------------------------------
try 
{
    var x = y / 0;
}
catch (Exception e) 
{
    ls.Log(e);}
}
```

## See Also
The following nuGet packages implement the LoggerService

* * PvWay.LoggerService.nc6
  * ConsoleLogger: Colorful console implementation
  * MuteLogger: Silent logger for uTesting
  * MicrosoftLogger: uses the Microsoft.Extensions.Logger

* PvWay.MsSqlLogWriter.nc6: Implementation for Ms SQL Database

* PvWay.PgSqlLogWriter.nc6: Implementation for PostgreSQL Database
