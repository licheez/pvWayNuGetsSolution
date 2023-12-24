using Microsoft.Extensions.Logging;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.StackTraceConsole.nc8;

public class CrashTester(ILoggerService ls)
{
    public void Crash()
    {
        CrashHere();
    }

    private void CrashHere()
    {
        try
        {
            throw new MissingFieldException("somme app exception");
        }
        catch (Exception e)
        {
            ls.LogCritical(e, "there was a problem");
        }
    }
}