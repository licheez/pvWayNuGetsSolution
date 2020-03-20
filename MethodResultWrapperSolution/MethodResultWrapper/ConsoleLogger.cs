using System;
using System.Collections.Generic;

namespace pvWay.MethodResultWrapper
{
    public class ConsoleLogger : ILoggerService
    {
        private string _userId;
        private string _companyId;
        public void SetUser(string userId, string companyId = null)
        {
            _userId = userId;
            _companyId = companyId;
        }

        public void Log(
            string message = "pass",
            SeverityEnum severity = SeverityEnum.Debug,
            string memberName = "",
            string filePath = "",
            int lineNumber = -1)
        {
            var line =
                $"sev: {severity}{Environment.NewLine}" +
                $"member: {memberName}{Environment.NewLine}" +
                $"file: {filePath}{Environment.NewLine}" +
                $"line: {lineNumber}{Environment.NewLine}" +
                $"message:{message}{Environment.NewLine}" +
                $"user: {_userId}{Environment.NewLine}" +
                $"company: {_companyId}{Environment.NewLine}";
            Console.WriteLine(line);
        }

        public void Log(
            IEnumerable<string> messages, 
            SeverityEnum severity, 
            string memberName = "", 
            string filePath = "",
            int lineNumber = -1)
        {
            var errorMessage = string.Empty;
            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    errorMessage += Environment.NewLine;
                errorMessage += message;
            }

            Log(errorMessage, severity, memberName, filePath, lineNumber);
        }

        public void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            string memberName = "",
            string filePath = "", 
            int lineNumber = -1)
        {
            Log(e.GetDeepMessage(), severity, memberName, filePath, lineNumber);
        }

        public void Log(
            IMethodResult res,
            string memberName = "",
            string filePath = "",
            int lineNumber = -1)
        {
            Log(res.ErrorMessage, res.Severity, memberName, filePath, lineNumber);
        }

        public void Dispose()
        {
            // nop
        }
    }
}
