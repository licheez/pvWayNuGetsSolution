using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.Hybrid.nc6;
using PvWay.LoggerService.PgSql.nc6;
using PvWay.LoggerService.SeriConsole.nc6;

// add a consoleLogWriter (here SeriConsole)
var services = new ServiceCollection();
services.AddPvWaySeriConsoleLogWriter();

// add a sql logWriter (here PgSql)
const string pCs = "Server=localhost;" +
                   "Database=postgres;" +
                   "User Id=postgres;" +
                   "Password=S0mePwd_;";
services.AddPvWayPgSqlLogWriter(
    _ => Task.FromResult<string>(pCs));

// Inject an hybrid logger service that will
// combine the ConsoleLogWriter and the SqlLogWriter
services.AddPvWayHybridLoggerService();

var sp = services.BuildServiceProvider();

var hLoggerService = sp.GetRequiredService<IHybridLoggerService>();

// the next method call will output on both the console and the database
await hLoggerService.LogAsync("Hello Hybrid Logger", SeverityEnu.Info);
