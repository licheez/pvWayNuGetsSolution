MsSQLLoggerService
******************

This service implements the ILoggerService from the
MethodResultWrapper nuGet package using an Ms Sql database table
as persistence layer

The service is implemented as a singleton

You can get an instance of the service by calling the 
static MsSqlLoggerService.GetInstance method

Pre-conditions for this service to work property
************************************************

The connection string provided should allow inserting rows
into a table that should conform to the following DDL.

Loggin table definition example
-------------------------------
CREATE TABLE [dbo].[Log] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](36) NULL,
	[CompanyId] [varchar](36) NULL,
	[SeverityCode] [char](1) NOT NULL,
	[MachineName] [varchar](50) NOT NULL,
	[Context] [varchar](256) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreateDateUtc] [datetime] NOT NULL,

	CONSTRAINT [PK_Log] 
		PRIMARY KEY CLUSTERED [Id] DESC
);

Columns
-------
 
Id
---
 The Id column is not required and will not be populted by the service. 
 However it might be convenient to have a primary column sorted DESC
 so that the last logs will be at the top of a select statement by
 default.
 If you define this column make sure the database will fill it accordingly
 by for example using the IDENTITY keyword

UserId (default column name)
----------------------------
 * You can provide your own column name for this column
 * The userId column persists the identification of the connected user if any.
 * This column should be nullable.
 * This column should be of type varchar
 * The logger will truncate any info exceding the max column length

CompanyId (default column name)
-------------------------------
 * You can provide your own column name for this column
 * The companyId column persists the identification of the company of the connected user if any.
 * This column should be nullable.
 * This column should be of type varchar
 * The logger will truncate any info exceding the max column length

SeverityCode (default column name)
----------------------------------
 * You can provide your own column name for this column
 * The SeverityCode column persists the MethodResultWrapper.SeverityEnum code
 * This column should be non nullable.
 * This column should be of type char (one char is enough)

MachineName (default column name)
---------------------------------
 * You can provide your own column name for this column
 * The CompanyName column persists Environment.MachineName.
 * This column should be non nullable.
 * This column should be of type varchar
 * The logger will truncate any info exceding the max column length

Context (default column name)
-----------------------------
 * You can provide your own column name for this column
 * The Context column persists class file, method name and code line number.
 * This column should be non nullable.
 * This column should be of type varchar
 * The logger will truncate any info exceding the max column length
 
Message (default column name)
-----------------------------
 * You can provide your own column name for this column
 * The Message column persists the message info.
 * This column should be non nullable.
 * This column should be of type nvarchar(MAX)

CreateDateUtc (default column name)
------------------------------------
 * You can provide your own column name for this column
 * The Message column persists the UTC date.
 * This column should be non nullable.
 * This column should be of type datetime

Usage
*****

See here after how to invoke the GetInstance method for
getting a (singleton) instance of the logger.

Example 1 : providing your own values for the schema, table and column names
----------------------------------------------------------------------------
private static void Main(string[] args)
{
    const string cs = "data source=Localhost;initial catalog=MyLoggingMsSqlDb;"
                    + "integrated security=True;MultipleActiveResultSets=True;";

    var ls = Logger.GetInstance(
        cs,
        SeverityEnum.Debug,
        "dbo", // table schema
        "Log", // table name
        "UserId", // userId column name
        "CompanyId", // companyId column name
        "MachineName", // machineName column name
        "SeverityCode", // severityCode column name
        "Context", // context column name
        "Message", // message column name
        "CreateDateUtc", // createDateUtc column name
        "me", // userId (can be set at construction time and updated later
        "myCompany" // companyId (can be set at construction time and updated later
		);

    ls.Log(new Exception());

    Console.WriteLine("hit enter to quit");
    Console.ReadLine();
}

Example 2 : using all default values
----------------------------------------------------------------------------
private static void Main(string[] args)
{
    const string cs = "data source=Localhost;initial catalog=MyLoggingMsSqlDb;"
                    + "integrated security=True;MultipleActiveResultSets=True;";

    var ls = Logger.GetInstance(
        cs,
        SeverityEnum.Debug);

    ls.Log(new Exception());

    Console.WriteLine("hit enter to quit");
    Console.ReadLine();
}

