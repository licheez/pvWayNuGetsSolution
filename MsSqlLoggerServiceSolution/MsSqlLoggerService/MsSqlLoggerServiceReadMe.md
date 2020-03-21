# MsSQLLoggerService
## Version 1.2.1

This service implements the ILoggerService from the **[MethodResultWrapper](https://www.nuget.org/packages/MethodResultWrapper/)** nuGet package using an Ms Sql database table as persistence layer


### Pre-conditions for this service to work property

The connection string provided should allow inserting rows
into a table that should conform to the following DDL.

### Log table definition example
``` sql

CREATE TABLE [dbo].[Log] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](36) NULL,
    [CompanyId] [varchar](36) NULL, 
	[SeverityCode] [char](1) NOT NULL, -- [D]Debug... [F] Fatal (see SeverityEnum)
	[MachineName] [varchar](50) NOT NULL, -- Environment.MachineName
	[Topic] varchar](50) NULL, -- enables to groupe log items
	[Context] [varchar](256) NOT NULL, -- membername, filepath, line number...
	[Message] [nvarchar](max) NOT NULL, -- the message
	[CreateDateUtc] [datetime] NOT NULL, -- timestamp in universal central time

	CONSTRAINT [PK_Log] 
		PRIMARY KEY CLUSTERED ([Id] DESC)
);


GO
CREATE NONCLUSTERED INDEX 
	ON TABLE [dbo].[Log] ([Topic] ASC)
    WHERE [Topic] IS NOT NULL
    
```
### Columns
 
#### Id

The Id column is not required and will not be populted by the service. However it might be convenient to have a primary column sorted DESC so that the last logs will be at the top of a select statement by default.

If you define this column make sure the database will fill it accordingly by for example using the IDENTITY keyword

#### UserId

* You can provide your own column name for this column
* The userId column persists the identification of the connected user if any
* This column should be nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### CompanyId

* You can provide your own column name for this column
* The companyId column persists the identification of the company of the connected user if any
* This column should be nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### SeverityCode

* You can provide your own column name for this column
* The SeverityCode column persists the MethodResultWrapper.SeverityEnum code
* This column should be non nullable
* This column should be of type char (one char is enough)

``` csharp
   // Sevirity enum and corresponding codes
   // -------------------------------------
   public enum SeverityEnum
    {
        Ok, // "O"
        Debug, // "D"
        Info, // "I"
        Warning, // "W"
        Error, // "E"
        Fatal, // "F"
    }
```

#### MachineName

*This column is certainly usefull in web farms*

* You can provide your own column name for this column
* The CompanyName column persists Environment.MachineName
* This column should be non nullable
* This column should be of type varchar
* The logger will truncate any info exceding the max column length

#### Context

* You can provide your own column name for this column
* The Context column persists method name, filepath and code line number.
* This column should be non nullable.
* This column should be of type varchar
* The logger will truncate any info exceding the max column length
 
#### Message

* You can provide your own column name for this column
* The Message column persists the message info.
* This column should be non nullable.
* This column should be of type nvarchar(MAX)

#### CreateDateUtc

* You can provide your own column name for this column
* The Message column persists the UTC date.
* This column should be non nullable.
* This column should be of type datetime

### Usage

```csharp
using System;
using pvWay.MethodResultWrapper;
using pvWay.MsSqlLoggerService;

namespace MsSqlLoggerServiceLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cs = "data source=Localhost;initial catalog=iota_PRD_20200208;" +
                              "integrated security=True;MultipleActiveResultSets=True;";

            var ls = new Logger(
                cs,
                SeverityEnum.Debug,
                "dbo",
                "Log",
                "UserId",
                "CompanyId",
                "MachineName",
                "SeverityCode",
                "Context",
                "Topic",
                "Message",
                "CreateDateUtc");

            ls.Log(new Exception());

            Console.WriteLine("hit enter to quit");
            Console.ReadLine();
        }
    }
}

```
