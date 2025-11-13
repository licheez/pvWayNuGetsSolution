using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.SeriConsole.nc6;

Console.WriteLine("Integration testing console for Serilog");

var inMemSettings = new Dictionary<string, string>
{
    // SERILOG
    { "PvWayLoggerServiceConfig:MinLogLevel", "trace" },
};

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(inMemSettings)
    .Build();

var services = new ServiceCollection();

services.TryAddPvWayLoggerServiceSeriService(config);

var sp = services.BuildServiceProvider();

var lp = sp.GetRequiredService<ILoggerProvider>();

var lpLogger = lp.CreateLogger("TestLogger");
lpLogger.Log(LogLevel.Information, "Hello from Serilog ms Logger");

var slLogger = sp.GetRequiredService<ILoggerService>();
await slLogger.LogAsync("Hello from ILoggerService!");
