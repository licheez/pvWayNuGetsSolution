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

The **AddPvWayPgSqlLoggerService** method extends the IServiceCollection

The default lifetime is **Singleton** and the default minimum log level is **Trace**... i.e. logging everything

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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public static class PvWayPgSqlLogger
{
    // CREATORS
    public static IPgSqlLogWriter CreateWriter(
        Func<SqlRoleEnu, Task<string>> getCsAsync,
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
        return new PgSqlLogWriter(
            new PgSqlConnectionStringProvider(getCsAsync),
            new PgSqlLogWriterConfig(
                schemaName, tableName,
                userIdColumnName, companyIdColumnName,
                machineNameColumnName, severityCodeColumnName,
                contextColumnName, topicColumnName,
                messageColumnName, createDateUtcColumnName));
    }
    
    public static IPgSqlLoggerService CreateService(
        IPgSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new PgSqlLoggerService(config, logWriter);
    }

    public static IPgSqlLoggerService<T> CreateService<T>(
        IPgSqlLogWriter logWriter,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        var config = new LoggerServiceConfig(minLogLevel);
        return new PgSqlLoggerService<T>(config, logWriter);
    }
    
    // LOG WRITER
    public static void AddPvWayPgSqlLogWriter(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null)
    {
        var csp = new PgSqlConnectionStringProvider(getCsAsync);
        var cfg = new PgSqlLogWriterConfig(lwConfig);
        var logWriter = new PgSqlLogWriter(csp, cfg); 
        services.TryAddSingleton<IPgSqlLogWriter>(_ => logWriter);
        services.TryAddSingleton<ISqlLogWriter>(_ => logWriter);
    }
    
    // FACTORY
    public static void AddPvWayPgSqlLoggerServiceFactory(
        this IServiceCollection services,
        IConfiguration config,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        SeverityEnu minLogLevel = SeverityEnu.Trace)
    {
        services.TryAddSingleton<ILoggerServiceFactory<IPgSqlLoggerService>>(
            _ =>
                new PgSqlLoggerServiceFactory(getCsAsync, config, minLogLevel));
    }
    
   // SERVICES
   /// <summary>
   /// Use this injector if you already injected the IPgSqlLogWriter
   /// </summary>
   /// <param name="services"></param>
   /// <param name="minLogLevel"></param>
   /// <param name="lifetime"></param>
   public static void AddPvWayPgSqlLoggerService(
       this IServiceCollection services,
       SeverityEnu minLogLevel = SeverityEnu.Trace,
       ServiceLifetime lifetime = ServiceLifetime.Singleton)
   {
       services.TryAddSingleton<ILoggerServiceConfig>(_ =>
           new LoggerServiceConfig(minLogLevel));
       
       RegisterService(services, lifetime);
        
       var sd = new ServiceDescriptor(
           typeof(IPgSqlLoggerService<>),
           typeof(PgSqlLoggerService<>),
           lifetime);
       services.Add(sd);
   }
   
    public static void AddPvWayPgSqlLoggerService(
        this IServiceCollection services,
        Func<SqlRoleEnu, Task<string>> getCsAsync,
        IConfiguration? lwConfig = null,
        SeverityEnu minLogLevel = SeverityEnu.Trace,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAddSingleton<ILoggerServiceConfig>(_ =>
            new LoggerServiceConfig(minLogLevel));
        
        services.AddPvWayPgSqlLogWriter(getCsAsync, lwConfig);
        
        RegisterService(services, lifetime);
    }
    
    private static void RegisterService(
        IServiceCollection services, ServiceLifetime lifetime)
    {
        var descriptors = new List<ServiceDescriptor>
        {
            new ServiceDescriptor(typeof(IMsSqlLoggerService),
                typeof(PgSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService),
                typeof(PgSqlLoggerService),
                lifetime),
            new ServiceDescriptor(typeof(IMsSqlLoggerService<>),
                typeof(PgSqlLoggerService<>),
                lifetime),
            new ServiceDescriptor(typeof(ISqlLoggerService<>),
                typeof(PgSqlLoggerService<>),
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

