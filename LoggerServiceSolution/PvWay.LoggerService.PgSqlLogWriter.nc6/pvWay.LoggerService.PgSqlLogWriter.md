# PostgreSQL Log Writer for .Net Core 6 by pvWay

## Dependendies
* pvWay.LoggerService.Abstractions.nc6
* Npgsql

## Description

This nuGet is a PostgreSQL implementation of the *PvWay.LoggerService.Abstractions.nc6* that will persist logs into a table in the PostgreSQL database. This implementation uses the light PostgreSQL DAO nuGet.


### Pre-conditions for this service to work properly

The connection string provided should allow inserting rows
into a table that should conform to the following DDL.

### Log table definition example
``` sql

-- Table: public.AppLog

-- DROP TABLE IF EXISTS public."AppLog";

CREATE TABLE IF NOT EXISTS public."AppLog"
(
    "Id" integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "UserId" character varying(36) COLLATE pg_catalog."default",
    "CompanyId" character varying(36) COLLATE pg_catalog."default",
    "SeverityCode" "char" NOT NULL,
    "MachineName" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Topic" character varying(50) COLLATE pg_catalog."default",
    "Context" character varying(256) COLLATE pg_catalog."default" NOT NULL,
    "Message" text COLLATE pg_catalog."default" NOT NULL,
    "CreateDateUtc" timestamp without time zone NOT NULL,
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
If you define this column make sure the database will fill it accordingly by for example using the IDENTITY keyword

#### UserId

* You can provide your own column name for this column
* The UserId column persists the identification of the connected user if any
* This column should be nullable
* This column should be of type character varying
* The logger will truncate any info exceding the max column length

#### CompanyId

* You can provide your own column name for this column
* The CompanyId column persists the identification of the company of the connected user if any
* This column should be nullable
* This column should be of type character varying
* The logger will truncate any info exceding the max column length

#### SeverityCode

* You can provide your own column name for this column
* The SeverityCode column persists the SeverityEnum code
* This column should be non nullable
* This column should be of type char (one char is enough)

``` csharp
   // Exemple of Sevirity enum and corresponding codes
   // ------------------------------------------------
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

#### MachineName

*This column is certainly usefull in web farms*

* You can provide your own column name for this column
* The MachineName column persists Environment.MachineName
* This column should be non nullable
* This column should be of type character varying
* The logger will truncate any info exceding the max column length

#### Topic

*This column lets you group logs for a given topic*

* You can provide your own column name for this column
* This column should be nullable
* This column should be of type character varying
* The logger will truncate any info exceding the max column length

#### Context

*Where does it happened*

* You can provide your own column name for this column
* The Context column persists method name, filepath and code line number
* This column should be non nullable
* This column should be of type character varying
* The logger will truncate any info exceding the max column length
 
#### Message

*What happened*

* You can provide your own column name for this column
* The Message column persists the message info
* This column should be non nullable.
* This column should be of type text

#### CreateDateUtc

*When does it happened*

* You can provide your own column name for this column
* The Message column persists the UTC date.
* This column should be non nullable.
* This column should be of type timestamp without time zone

### Usage

```csharp
using PvWay.LoggerService.PgSqlLogWriter.nc6;

Console.WriteLine("Hello, PostgreSQL LoggerService");
Console.WriteLine("-------------------------------");
Console.WriteLine();

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=sa;" +
                       "Password=S0mePwd_;";

var pgSqlLogger = PgSqlLogWriter.FactorLoggerService(
     async () => 
         await Task.FromResult(pgSqlCs));

Console.WriteLine("logging using PostgreSQL");
await msSqlLogger.LogAsync("some debug");

Console.WriteLine("done");

```
Happy coding
