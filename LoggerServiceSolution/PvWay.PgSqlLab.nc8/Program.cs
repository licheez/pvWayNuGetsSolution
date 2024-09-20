using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.PgSql.nc8.Di;

Console.WriteLine("Hello, PgSqlLoggerService");
Console.WriteLine();

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=postgres;" +
                       "Password=S0mePwd_;";

var services = new ServiceCollection();
services.AddPvWayPgSqlLogWriter( 
    async _ => await Task.FromResult(pgSqlCs),
    "pgSqlLogger");
services.AddPvWayPgSqlLoggerService();
var sp = services.BuildServiceProvider();
var logger = sp.GetService<ISqlLoggerService>()!;

await logger.LogAsync("This is a trace test log message", SeverityEnu.Trace);
await logger.LogAsync("This is a debug test log message");
await logger.LogAsync("This is an info test log message", SeverityEnu.Info);
await logger.LogAsync("This is a warning test log message", SeverityEnu.Warning);
await logger.LogAsync("This is an error test log message", SeverityEnu.Error);
await logger.LogAsync("This is a fatal test log message", SeverityEnu.Fatal);

logger.Log(LogLevel.Trace, "MsLog trace");
var sooner = TimeSpan.FromSeconds(-1);
var purgePlan = new Dictionary<SeverityEnu, TimeSpan>
{
    { SeverityEnu.Ok, sooner },
    { SeverityEnu.Trace, sooner },
    { SeverityEnu.Debug, sooner },
    { SeverityEnu.Info, sooner },
    { SeverityEnu.Warning, sooner },
    { SeverityEnu.Error, sooner },
    { SeverityEnu.Fatal, sooner }
};
var ra = await logger.PurgeLogsAsync(purgePlan);
Console.WriteLine($"{ra} rows were purged");


