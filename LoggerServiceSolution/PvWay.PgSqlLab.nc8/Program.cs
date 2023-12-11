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

