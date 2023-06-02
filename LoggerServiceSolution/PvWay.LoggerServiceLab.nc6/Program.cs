using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.MsSqlLogWriter.nc6;
using PvWay.LoggerService.nc6;
using PvWay.LoggerService.PgSqlLogWriter.nc6;

Console.WriteLine("Hello, LoggerService");
Console.WriteLine("--------------------");
Console.WriteLine();

var ls = new ConsoleLogger();

await ls.LogAsync("some Debug");
await ls.LogAsync("some Info", SeverityEnum.Info);
await ls.LogAsync("some Warning", SeverityEnum.Warning);
await ls.LogAsync("some Error", SeverityEnum.Error);
await ls.LogAsync("some Fatal", SeverityEnum.Fatal);
await ls.LogAsync("it's ok", SeverityEnum.Ok);

const string msSqlCs = "Data Source=localhost;" +
                       "Initial Catalog=iota800_dev;" +
                       "integrated security=True;" +
                       "MultipleActiveResultSets=True;" +
                       "TrustServerCertificate=True;";

var msSqlLogger = MsSqlLogWriter.FactorLoggerService(
    async () =>
        await Task.FromResult(msSqlCs));
Console.WriteLine("logging using MsSql");
await msSqlLogger.LogAsync("some debug");
Console.WriteLine("done");

const string pgSqlCs = "Server=localhost;" +
                       "Database=postgres;" +
                       "User Id=sa;" +
                       "Password=S0mePwd_;";

var pgSqlLogger = PgSqlLogWriter.FactorLoggerService(
     async () => 
         await Task.FromResult(pgSqlCs));
await pgSqlLogger.LogAsync("some debug");
Console.WriteLine("logging using PostgreSQL");
await msSqlLogger.LogAsync("some debug");
Console.WriteLine("done");
