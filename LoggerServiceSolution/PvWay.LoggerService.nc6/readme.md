# pvWay Logger Service for dotNet core 6

## Description
This nuget provides the base classes for several very intuitive LoggerService implementations of the *PvWay.LoggerService.Abstractions.nc8* ILoggerService interface :

* **ConsoleLoggerService** (IConsoleLoggerService)
  * This colorful implementation uses Console.WriteLine outputting logs to the standard out. 


* **MsSqlLoggerService** (IMsSqlLoggerService)
  * This implementation uses a DAO connection towards a Ms Sql Server Database that persist log rows into the table of your choice


* **MuteLoggerService** (IMuteLoggerService)
  * As the name sounds this implementation can be used to injecting a silent logger. This can be handy for unit testing.


* **PgSqlLoggerService** (IPgSqlLoggerService)
  * This implementation uses a DAO connection towards a PostgreSQL Database that persist log rows into the table of your choice


* **SeriConsoleLoggerService** (ISeriConsoleLoggerService)
    * Uses the well known serilog(tm) console skin package


* **UTestLoggerService** (IUTestLoggerService)
    * Unit testing implementation allowing you to perform asserts on logs content


