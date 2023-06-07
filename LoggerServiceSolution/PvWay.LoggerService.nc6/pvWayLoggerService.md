# pvWay Logger Service for dotNet core 6

## Dependendies
* pvWay.LoggerService.Abstractions.nc6
* Microsoft.Extensions.Logging.Abstraction

## Description
This nuget provides several very intuitive LoggerService implementations of the *PvWay.LoggerService.Abstractions.nc6* ILoggerService interface :
* ConsoleLogger
* MuteLogger
* Microsoft Logger
* PersistenceLogger

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
