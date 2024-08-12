// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;
using PvWay.LoggerServiceDebug.nc6;

Console.WriteLine("Debugging LoggerService!");

ILogger logger = new DebugLoggerService();

var ex = new Exception("mainEx", new Exception("innerEx"));
try
{
    var eventId = new EventId(1024, "TestEvent");
    logger.Log(LogLevel.Critical, eventId, "anyMessage", ex,
        (message, exception) => $"got '{message}' and '{exception}'");
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
    
Console.WriteLine("now using logCritical");
logger.LogCritical(ex, "got {Error}", ex);