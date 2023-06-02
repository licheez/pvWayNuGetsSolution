namespace PvWay.LoggerService.nc6;

public static class ExceptionExtension
{
    public static string GetDeepMessage(this Exception e)
    {
        var message = RecursiveDeepMessage(e);
        var stackTrace = e.StackTrace;
        return $"Exception: {message}{Environment.NewLine}StackTrace: {stackTrace}";
    }

    private static string RecursiveDeepMessage(Exception e)
    {
        var message = e.Message;
        if (e.InnerException != null)
            message += Environment.NewLine
                       + RecursiveDeepMessage(e.InnerException);
        return message;
    }
}