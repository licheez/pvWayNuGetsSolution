using PvWay.LoggerService.nc6;
using PvWay.LoggerServiceLab.nc6;

Console.WriteLine("Hello, LoggerService");
Console.WriteLine("--------------------");
Console.WriteLine();

Console.WriteLine("testing the ConsoleLogger");
await ConsoleLoggerDemo.FactorAndLogAsync();
await ConsoleLoggerDemo.InjectAndLogAsync();

Console.WriteLine("testing the MuteLogger");
await MuteLoggerDemo.FactorAndLogAsync();
await MuteLoggerDemo.InjectAndLogAsync();

Console.WriteLine("testing the MsLogger");
await MsLoggerDemo.FactorAndLogAsync();
await MsLoggerDemo.InjectAndLogAsync();

Console.WriteLine("testing the MsConsoleLogger");
await MsConsoleLoggerDemo.FactorAndLog();
await MsConsoleLoggerDemo.InjectAndLog();

Console.WriteLine("testing the MsSqlLogger");
await MsSqlLoggerDemo.FactorAndLogAsync();
await MsSqlLoggerDemo.InjectAndLog();

Console.WriteLine("testing the PgSqlLogger");
await PqSqlLoggerDemo.FactorAndLogAsync();
await PqSqlLoggerDemo.InjectAndLogAsync();

Console.WriteLine("testing the MethodResultWrapper");
var ls = PvWayLoggerService.CreateConsoleLoggerService();
var userStore = new UserStore();
var mrDemo = new MethodResultWrapperDemo(ls, userStore);
await mrDemo.GetUserFirstNameAsync("pierre");

Console.WriteLine("done");
