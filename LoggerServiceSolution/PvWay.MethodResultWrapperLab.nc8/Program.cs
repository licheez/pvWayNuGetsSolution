using Microsoft.Extensions.DependencyInjection;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.SeriConsole.nc8;
using PvWay.MethodResultWrapperLab.nc8;

Console.WriteLine("Hello, World!");

Console.WriteLine("Hello, ConsoleLoggerService");
Console.WriteLine();

var services = new ServiceCollection();
// let's inject the Serilog console logger service
services.AddPvWaySeriConsoleLoggerService();

// we need to inject the UserStore and the MethodResultWrapperDemo as well
services.AddTransient<IUserStore, UserStore>();
services.AddTransient<MethodResultWrapperDemo>();

var sp = services.BuildServiceProvider();

var mrwDemo = sp.GetService<MethodResultWrapperDemo>()!;
var res  = await mrwDemo.GetUserFirstNameAsync("john");
if (res.Failure)
{
    var serilogService = sp.GetService<ILoggerService>()!;
    await serilogService.LogAsync(res);
}
