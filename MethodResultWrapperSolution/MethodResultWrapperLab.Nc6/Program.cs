using PvWay.LoggerService.nc6;
using pvWay.MethodResultWrapper.nc6;

var cLog = PvWayLoggerService.CreateConsoleLoggerService();


cLog.SetTopic("the topic");
cLog.SetUser("the user", "the company");
await cLog.LogAsync("test");

try
{
    throw new Exception("test");
}
catch (Exception e)
{
    await cLog.LogAsync(e);
}

var res = MethodResult<string>.Null;
Console.WriteLine(res.Data == null);