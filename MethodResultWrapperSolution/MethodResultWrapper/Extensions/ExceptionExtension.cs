using System;

namespace pvWay.MethodResultWrapper.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetDeepMessage(this Exception e)
        {
            var message = _getDeepMessage(e);
            var stackTrace = e.StackTrace;
            return $"Exception: {message}{Environment.NewLine}StackTrace: {stackTrace}";
        }

        private static string _getDeepMessage(Exception e)
        {
            var message = e.Message;
            if (e.InnerException != null)
                message += Environment.NewLine
                           + _getDeepMessage(e.InnerException);
            return message;
        }
    }
}