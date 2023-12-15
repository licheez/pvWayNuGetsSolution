# pvWay PgSqlLoggerService for dotNet Core 8

## Description
This nuget implements the ILoggerService using a DAO connection towards an **PostgreSQL** Database. 

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

## Injection

The **AddPvWayPgSqlLoggerService** method extends the IServiceCollection

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

namespace PvWay.LoggerService.PgSql.nc8;

public class PgSqlLogWriterConfig(
    string? schemaName = "public",
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
    public string SchemaName { get; } = schemaName ?? "public";
    public string TableName { get; } = tableName ?? "Log";
    public string UserIdColumnName { get; } = userIdColumnName ?? "UserId";
    public string CompanyIdColumnName { get; } = companyIdColumnName ?? "CompanyId";
    public string MachineNameColumnName { get; } = machineNameColumnName ?? "MachineName";
    public string SeverityCodeColumnName { get; } = severityCodeColumnName ?? "SeverityCode";
    public string ContextColumnName { get; } = contextColumnName ?? "Context";
    public string TopicColumnName { get; } = topicColumnName ?? "Topic";
    public string MessageColumnName { get; } = messageColumnName ?? "Message";
    public string CreateDateUtcColumnName { get; } = createDateUtcColumnName ?? "CreateDateUtc";

    public PgSqlLogWriterConfig(IConfiguration? config):
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
    public static void AddPvWayPgSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));

        services.AddSingleton<IPgSqlLogWriter>(_ =>
            new PgSqlLogWriter(
                new PgSqlConnectionStringProvider(getCsAsync),
                new PgSqlLogWriterConfig(lwConfig)));

        var sd = new ServiceDescriptor(
            typeof(ILoggerService),
            typeof(PgSqlLoggerService),
            lifetime);
        services.Add(sd);
        
        var sd2 = new ServiceDescriptor(
            typeof(IPgSqlLoggerService),
            typeof(PgSqlLoggerService),
            lifetime);
        services.Add(sd2);
    }
```

## Static factories

The **PvWayPgSqlLogger** static class also exposes two public **Create** methods enabling to factor the service from your own code

``` csharp
    public static IPgSqlLoggerService Create(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        string? schemaName = "public",
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
        return new PgSqlLoggerService(
            new PgSqlLogWriter(
                new PgSqlConnectionStringProvider(getCsAsync), 
                new PgSqlLogWriterConfig(
                    schemaName, tableName, 
                    userIdColumnName, companyIdColumnName, 
                    machineNameColumnName, severityCodeColumnName, 
                    contextColumnName, topicColumnName, 
                    messageColumnName, createDateUtcColumnName)), 
            new LoggerServiceConfig(minLogLevel));
    }

    public static IPgSqlLoggerService<T> Create<T>(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        string? schemaName = "public",
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
        return new PgSqlLoggerService<T>(
            new PgSqlLogWriter(
                new PgSqlConnectionStringProvider(getCsAsync), 
                new PgSqlLogWriterConfig(
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
using PvWay.LoggerService.PgSql.nc8;

Console.WriteLine("Hello, PgSqlLoggerService");
Console.WriteLine();

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=postgres;" +
                       "Password=S0mePwd_;";

var services = new ServiceCollection();
services.AddPvWayPgSqlLoggerService(_ => 
    Task.FromResult(pgSqlCs));
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

### Log table definition example
``` sql

-- Table: public.AppLog

-- DROP TABLE IF EXISTS public."AppLog";

CREATE TABLE IF NOT EXISTS public."AppLog"
(
    "Id" integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "UserId" character varying(36) COLLATE pg_catalog."default",
    "CompanyId" character varying(36) COLLATE pg_catalog."default",
    "SeverityCode" character (1) COLLATE pg_catalog."default" NOT NULL,
    "MachineName" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Topic" character varying(50) COLLATE pg_catalog."default",
    "Context" character varying(256) COLLATE pg_catalog."default" NOT NULL,
    "Message" text COLLATE pg_catalog."default" NOT NULL,
    "CreateDateUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "ApplicationLog_pkey" PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."AppLog"
    OWNER to postgres;

-- Index: ApplicationLog_IX_Topic

-- DROP INDEX IF EXISTS public."ApplicationLog_IX_Topic";

CREATE INDEX IF NOT EXISTS "ApplicationLog_IX_Topic"
    ON public."AppLog" USING btree
    ("Topic" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
    
```
### Columns

#### Id

The Id column is not required and will not be populted by the service.
However it might be convenient to have a numeric primary column.
If you define this column make sure the database will fill it accordingly by for example using the IDENTITY syntax

#### UserId

* You can provide your own column name for this column
* The UserId column persists the identification of the connected user if any
* This column should be nullable
* This column should be of type *character varying*
* The logger will truncate any info exceding the max column length

#### CompanyId

* You can provide your own column name for this column
* The CompanyId column persists the identification of the company of the connected user if any
* This column should be nullable
* This column should be of type *character varying*
* The logger will truncate any info exceding the max column length

#### SeverityCode

* You can provide your own column name for this column
* The SeverityCode column persists the SeverityEnum code
* This column should be non nullable
* This column should be of type *characater* (one char is enough)

``` csharp
   // Exemple of Sevirity enum and corresponding codes
   // ------------------------------------------------
   public enum SeverityEnum
    {
        Ok,         // "O"
        Trace,      // "T"
        Debug,      // "D"
        Info,       // "I"
        Warning,    // "W"
        Error,      // "E"
        Fatal,      // "F"
    }
```

#### MachineName

*This column is certainly usefull in web farms*

* You can provide your own column name for this column
* The MachineName column persists Environment.MachineName
* This column should be non nullable
* This column should be of type *character varying*
* The logger will truncate any info exceeding the max column length

#### Topic

*This column lets you group logs for a given topic*

* You can provide your own column name for this column
* This column should be nullable
* This column should be of type *character varying*
* The logger will truncate any info exceeding the max column length

#### Context

*Where does it happened*

* You can provide your own column name for this column
* The Context column persists method name, filepath and code line number
* This column should be non nullable
* This column should be of type *character varying*
* The logger will truncate any info exceeding the max column length

#### Message

*What happened*

* You can provide your own column name for this column
* The Message column persists the message info
* This column should be non nullable.
* This column should be of type *text*

#### CreateDateUtc

*When does it happened*

* You can provide your own column name for this column
* The Message column persists the UTC date.
* This column should be non nullable.
* This column should be of type *timestamp with time zone*

