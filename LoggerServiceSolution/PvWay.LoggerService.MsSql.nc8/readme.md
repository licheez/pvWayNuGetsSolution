# pvWay MsSqlLoggerService for dotNet Core 8

## Description
This nuget implements the ILoggerService using a DAO connection towards an Ms Sql Server Database. 

This implementation is compliant with the concept of dynamic credentials as the connection string is actually provided via an injectable method.

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

## Maintenance

For keeping the size of the log table under control there is a method that purges the logs based on a purge plan.

Task&lt;int&gt; PurgeLogsAsync(IDictionary&lt;SeverityEnu, TimeSpan&gt; retainDic);

The method returns the number of rows purged from the database.

Up to you to decide how long you need to keep the logs. This can be done per severity.

for example:
* Fatal: 6 month
* Error: 3 month
* Warning: 1 month
* Info: 5 days
* Debut: 1 day,
* Trace: 2 hours


## Injection

The **AddPvWayMsSqlLoggerService** method extends the IServiceCollection

The default lifetime is **Scoped** and the default minimum log level is **Trace**... i.e. logging everything

The injection method expects some important parameters

#### Func&lt;SqlRoleEnu, Task&lt;string&gt;&gt; getCsAsync

You'll need to provide a function that will get the connection string for the role.

There are 3 possible roles **Owner, Application, Reader**

* Owner should be able to update the db schema
* Application is the default role that can read/write
* Reader should only be able to read the db

Notice that the method is async

#### IConfiguration? lwConfig = null

This is typically referring to a section of the configuration that specifies the log table parameters such as
* schema name: defaulting to "dbo"
* table name: defaulting to "Log"

see here after the config class

``` csharp
using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

public class MsSqlLogWriterConfig(
    string? schemaName = "dbo",
    string? tableName = "Log",
    string? userIdColumnName = "UserId",
    string? companyIdColumnName = "CompanyId",
    string? machineNameColumnName = "MachineName",
    string? severityCodeColumnName = "SeverityCode",
    string? contextColumnName = "Context",
    string? topicColumnName = "Topic",
    string? messageColumnName = "Message",
    string? createDateUtcColumnName = "CreateDateUtc")
    : ISqlLogWriterConfig
{
    public string SchemaName { get; } = schemaName ?? "dbo";
    public string TableName { get; } = tableName ?? "Log";
    public string UserIdColumnName { get; } = userIdColumnName ?? "UserId";
    public string CompanyIdColumnName { get; } = companyIdColumnName ?? "CompanyId";
    public string MachineNameColumnName { get; } = machineNameColumnName ?? "MachineName";
    public string SeverityCodeColumnName { get; } = severityCodeColumnName ?? "SeverityCode";
    public string ContextColumnName { get; } = contextColumnName ?? "Context";
    public string TopicColumnName { get; } = topicColumnName ?? "Topic";
    public string MessageColumnName { get; } = messageColumnName ?? "Message";
    public string CreateDateUtcColumnName { get; } = createDateUtcColumnName ?? "CreateDateUtc";

    public MsSqlLogWriterConfig(IConfiguration? config):
        this(
            config?["schemaName"],
            config?["tableName"],
            config?["userIdColumnName"],
            config?["companyIdColumnName"],
            config?["machineNameColumnName"],
            config?["severityCodeColumnName"],
            config?["contextColumnName"],
            config?["topicColumnName"],
            config?["messageColumnName"],
            config?["createDateUtcColumnName"])
    {
    }
}
```

Finally you'll find here after the code for the service injection 

``` csharp
    public static void AddPvWayMsSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<IMsSqlLogWriter>(_ => 
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync),
                new MsSqlLogWriterConfig(lwConfig)));

        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd);

        var sd2 = new ServiceDescriptor(
            typeof(IMsSqlLoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd2);
        
        var sd3 = new ServiceDescriptor(
            typeof(ISqlLoggerService),
            typeof(MsSqlLoggerService),
            lifetime);
        services.Add(sd3);        
    }
```

## Static factories

The **PvWayMsSqlLogger** static class also exposes two public **Create** methods enabling to factor the service from your own code

``` csharp
    public static IMsSqlLoggerService Create(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        string? schemaName = "dbo",
        string? tableName = "Log",
        string? userIdColumnName = "UserId",
        string? companyIdColumnName = "CompanyId",
        string? machineNameColumnName = "MachineName",
        string? severityCodeColumnName = "SeverityCode",
        string? contextColumnName = "Context",
        string? topicColumnName = "Topic",
        string? messageColumnName = "Message",
        string? createDateUtcColumnName = "CreateDateUtc")
    {
        return new MsSqlLoggerService(
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync), 
                new MsSqlLogWriterConfig(
                    schemaName, tableName, 
                    userIdColumnName, companyIdColumnName, 
                    machineNameColumnName, severityCodeColumnName, 
                    contextColumnName, topicColumnName, 
                    messageColumnName, createDateUtcColumnName)), 
            new LoggerServiceConfig(minLogLevel));
    }

    public static IMsSqlLoggerService<T> Create<T>(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        string? schemaName = "dbo",
        string? tableName = "Log",
        string? userIdColumnName = "UserId",
        string? companyIdColumnName = "CompanyId",
        string? machineNameColumnName = "MachineName",
        string? severityCodeColumnName = "SeverityCode",
        string? contextColumnName = "Context",
        string? topicColumnName = "Topic",
        string? messageColumnName = "Message",
        string? createDateUtcColumnName = "CreateDateUtc")
    {
        return new MsSqlLoggerService<T>(
            new MsSqlLogWriter(
                new MsSqlConnectionStringProvider(getCsAsync), 
                new MsSqlLogWriterConfig(
                    schemaName, tableName, 
                    userIdColumnName, companyIdColumnName, 
                    machineNameColumnName, severityCodeColumnName, 
                    contextColumnName, topicColumnName, 
                    messageColumnName, createDateUtcColumnName)), 
            new LoggerServiceConfig(minLogLevel));
    }
```


## Usage

``` csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.MsSql.nc8;

Console.WriteLine("Hello, MsSqlLoggerService");
Console.WriteLine();

const string msSqlCs = "Data Source=localhost;" +
                       "Initial Catalog=NuGetDemo;" +
                       "integrated security=True;" +
                       "MultipleActiveResultSets=True;" +
                       "TrustServerCertificate=True;";

var services = new ServiceCollection();
services.AddPvWayMsSqlLoggerService(_ => 
    Task.FromResult(msSqlCs));
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

## SQL considerations

``` sql

    CREATE TABLE [dbo].[AppLog] (

	    [Id]		INT	IDENTITY(1,1) NOT NULL,
	    [UserId]	        VARCHAR(36) NULL,
	    [CompanyId]		VARCHAR(36) NULL, 
	    [SeverityCode]	CHAR(1) NOT NULL, -- [D]Debug... [F] Fatal (see SeverityEnum)
	    [MachineName]	VARCHAR(50) NOT NULL, -- Environment.MachineName
	    [Topic]			VARCHAR(50) NULL, -- enables to group log items for a given Topic
	    [Context]		VARCHAR(256) NOT NULL, -- concats membername, filepath, line number...
	    [Message]		NVARCHAR(MAX) NOT NULL, -- the message
	    [CreateDateUtc] DATETIME NOT NULL, -- timestamp in universal central time

	    CONSTRAINT [PK_Log] 
		    PRIMARY KEY CLUSTERED ([Id] DESC)
    );

    GO
    CREATE NONCLUSTERED INDEX [IX_TOPIC]
	    ON [dbo].[ApplicationLog] ([Topic] ASC)
        WHERE [Topic] IS NOT NULL;
    
```
### Columns

#### Id

The Id column is not required and will not be populated by the service.
However it might be convenient to have a numeric primary column sorted DESC so that the last logs will be at the top of a select statement by default.
If you define this column make sure the database will fill it accordingly by for example using the IDENTITY keyword

#### UserId

* You can provide your own column name for this column
* The UserId column persists the identification of the connected user if any
* This column should be nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### CompanyId

* You can provide your own column name for this column
* The CompanyId column persists the identification of the company of the connected user if any
* This column should be nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### SeverityCode

* You can provide your own column name for this column
* The SeverityCode column persists the MethodResultWrapper.SeverityEnum code
* This column should be non nullable
* This column should be of type char (one char is enough)

``` csharp
   // Exemple of Sevirity enum and corresponding codes
   // -------------------------------------
   public enum SeverityEnum
    {
        Ok, // "O"
        Trace, // "T"
        Debug, // "D"
        Info, // "I"
        Warning, // "W"
        Error, // "E"
        Fatal, // "F"
    }
```

#### MachineName

*This column is certainly useful in web farms*

* You can provide your own column name for this column
* The MachineName column persists Environment.MachineName
* This column should be non nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### Topic

*This column lets you group logs for a given topic*

* You can provide your own column name for this column
* This column should be nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### Context

*Where does it happened*

* You can provide your own column name for this column
* The Context column persists method name, filepath and code line number.
* This column should be non nullable.
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### Message

*What happened*

* You can provide your own column name for this column
* The Message column persists the message info
* This column should be non nullable.
* This column should be of type nvarchar(MAX)

#### CreateDateUtc

*When does it happened*

* You can provide your own column name for this column
* The Message column persists the UTC date.
* This column should be non nullable.
* This column should be of type datetime
