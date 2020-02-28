using System;

namespace pvWay.MethodResultWrapper
{
    public static class ExceptionExtension
    {
        public static string GetDeepMessage(this Exception e)
        {
            var message = e.Message
                          + Environment.NewLine
                          + e.StackTrace;
            if (e.InnerException != null)
                message += Environment.NewLine
                           + e.InnerException.GetDeepMessage();
            return message;
        }
    }
}
