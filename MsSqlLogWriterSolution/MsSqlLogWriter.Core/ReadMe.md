# Ms SQL Log Writer for .Net Core by pvWay

This nuGet exposes two log writer methods (one sync and one async) for persisting rich log rows into an Ms Sql database. 
This can be used in conjunction with the **[pvWay.MethodResultWrapper.Core](https://www.nuget.org/packages/MethodResultWrapper.Core/)** nuGet package for enabling this package to persist
logs into an Ms Sql Database


### Pre-conditions for this service to work properly

The connection string provided should allow inserting rows
into a table that should conform to the following DDL.

### Log table definition example
``` sql

    CREATE TABLE [dbo].[ApplicationLog] (

	    [Id]			INT			IDENTITY(1,1) NOT NULL,
	    [UserId]		VARCHAR(36) NULL,
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

The Id column is not required and will not be populted by the service. 
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

### Usage

```csharp
using pvWay.MethodResultWrapper.Core;
using pvWay.MsSqlLogWriter.Core;

namespace MsSqlLogWriterLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cs = "data source=Localhost;initial catalog=iota700_dev;" +
                              "integrated security=True;MultipleActiveResultSets=True;";

            // let's start by creating the LogWriter
            // using all default values for table and columns names
            var msSqlLogWriter = MsSqlLogWriterSingleton.GetInstance(cs);
            
            // let's now instantiate the PersistenceLogger class
            // from the pvWay.MethodResultWrapper.Core nuGet
            // The constructor for this class needs 3 params
            // (1) the dispose method of the logWriter
            // (2) a tuple delegate with named params for the WriteLog method
            // (3) a tuple delegate with named params for the WriteLogAsync method
            var persistenceLogger = new PersistenceLogger(
                msSqlLogWriter.Dispose,
                p => msSqlLogWriter.WriteLog(
                    p.userId, p.companyId, p.topic,
                    p.severityCode, p.machineName,
                    p.memberName, p.filePath, p.lineNumber,
                    p.message, p.dateUtc),
                async p => await msSqlLogWriter.WriteLogAsync(
                    p.userId, p.companyId, p.topic,
                    p.severityCode, p.machineName,
                    p.memberName, p.filePath, p.lineNumber,
                    p.message, p.dateUtc
                    ));
            
            // set some values for userId, companyId, and topic
            // so that the log columns will have some data
            persistenceLogger.SetUser("UserId", "CompanyId");
            persistenceLogger.SetTopic("Topic");

            // fire the pvWay.MethodResultWrapper PersistenceLogger.Log method
            // that will under the cover call the (sync) delegate
            // msSqlLogWriter.WriteLog method passed in the constructor
            // with the appropriate parameters
            persistenceLogger.Log("Hello Log");

            // this last line of code will generate an entry in the Log table
        }
    }
}

```
Happy coding
