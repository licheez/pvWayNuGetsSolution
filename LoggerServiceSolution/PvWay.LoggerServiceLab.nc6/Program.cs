using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;
using PvWay.LoggerService.PgSqlLogWriter.nc6;

Console.WriteLine("Hello, LoggerService");
Console.WriteLine("--------------------");
Console.WriteLine();

var msLog = LoggerFactory
    .Create(builder => builder.AddConsole())
    .CreateLogger("Program");
var msLs = PvWayLoggerService.CreateMsLoggerService(msLog);
var e = new Exception("this is the exception from the MsLoggerService");
msLs.Log(e);
Console.WriteLine();

var msConsoleLs = PvWayLoggerService.CreateMsConsoleLoggerService();
msConsoleLs.Log("I'm The Ms Console Logger", SeverityEnum.Ok);
msConsoleLs.Log("I'm The Ms Console Logger");
msConsoleLs.Log("I'm The Ms Console Logger", SeverityEnum.Info);
msConsoleLs.Log("I'm The Ms Console Logger", SeverityEnum.Warning);
msConsoleLs.Log("I'm The Ms Console Logger", SeverityEnum.Error);
msConsoleLs.Log("I'm The Ms Console Logger", SeverityEnum.Fatal);
Console.WriteLine();

var muteLs = PvWayLoggerService.CreateMuteLoggerService();
muteLs.Log("The sound of silence");
Console.WriteLine();

var consoleLs = PvWayLoggerService.CreateConsoleLoggerService();

await consoleLs.LogAsync("some Debug");
await consoleLs.LogAsync("some Info", SeverityEnum.Info);
await consoleLs.LogAsync("some Warning", SeverityEnum.Warning);
await consoleLs.LogAsync("some Error", SeverityEnum.Error);
await consoleLs.LogAsync("some Fatal", SeverityEnum.Fatal);
await consoleLs.LogAsync("it's Ok", SeverityEnum.Ok);

//const string msSqlCs = "Data Source=localhost;" +
//                       "Initial Catalog=iota800_dev;" +
//                       "integrated security=True;" +
//                       "MultipleActiveResultSets=True;" +
//                       "TrustServerCertificate=True;";

//var msSqlLogger = MsSqlLogWriter.FactorLoggerService(
//    async () =>
//        await Task.FromResult(msSqlCs));
//Console.WriteLine("logging using MsSql");
//await msSqlLogger.LogAsync("some debug");
//Console.WriteLine("done");

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=sa;" +
                       "Password=S0mePwd_;";


var services = new ServiceCollection();

services.AddPvWayPgLogServices(
    ServiceLifetime.Transient,
    async () =>
        await Task.FromResult(pgSqlCs));
var sp = services.BuildServiceProvider();

var pgSqlLoggerService = sp.GetService<IPvWayPostgreLoggerService>()!;

Console.WriteLine("logging using PostgreSQL");
await pgSqlLoggerService.LogAsync("some debug");
Console.WriteLine("done");
