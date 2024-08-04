//using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.Console.nc6;
using PvWay.LoggerService.Hybrid.nc6;
using PvWay.LoggerService.MsSql.nc6;
using PvWay.LoggerService.PgSql.nc6;
using PvWay.LoggerService.SeriConsole.nc6;

Console.WriteLine("Hello, World!");

var cLw = PvWayConsoleLogger.CreateWriter();
var sLw = PvWaySerilogConsoleLogger.CreateWriter();

const string pCs = "Server=localhost;" +
                   "Database=postgres;" +
                   "User Id=postgres;" +
                   "Password=S0mePwd_;";
var pLw = PvWayPgSqlLogger.CreateWriter(
    _ => Task.FromResult(pCs));

const string mCs = "Data Source=localhost;" +
                   "Initial Catalog=NuGetDemo;" +
                   "integrated security=True;" +
                   "MultipleActiveResultSets=True;" +
                   "TrustServerCertificate=True;";
var mLw = PvWayMsSqlLogger.CreateWriter(
    _ => Task.FromResult(mCs));

var hLogger = PvWayHybridLogger.CreateService(
    SeverityEnu.Trace, cLw, sLw, pLw, mLw);

hLogger.Log("Hello", SeverityEnu.Info);
hLogger.LogInformation("some info");

var services = new ServiceCollection();
services.AddPvWaySeriConsoleLoggerService();
var sp = services.BuildServiceProvider();
var sLs = sp.GetRequiredService<IConsoleLoggerService>();
sLs.Log("ILoggerService");




