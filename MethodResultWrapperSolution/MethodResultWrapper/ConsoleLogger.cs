using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument

namespace pvWay.MethodResultWrapper
{
    public class ConsoleLogger : ILoggerService
    {
        private string _userId;
        private string _companyId;
        private string _topic;

        public void Dispose()
        {
            // nop
        }

        public void SetUser(
            string userId,
            string companyId = null)
        {
            _userId = userId;
            _companyId = companyId;
        }

        public void SetTopic(string topic)
        {
            _topic = topic;
        }

        public void Log(
            string message,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            Log(message, _topic, severity, memberName, filePath, lineNumber);
        }

        public void Log(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            Log(messages, _topic, severity, memberName, filePath, lineNumber);
        }

        public void Log(
            IMethodResult result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            Log(result, _topic, memberName, filePath, lineNumber);
        }

        public void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            Log(e, _topic, severity, memberName, filePath, lineNumber);
        }
        

        public void Log(
            string message,
            string topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            var line = 
                $"severity: {severity}{Environment.NewLine}" +
                $"memberName: {memberName}{Environment.NewLine}" +
                $"filePath: {filePath}{Environment.NewLine}" +
                $"lineNumber: {lineNumber}{Environment.NewLine}" +
                $"topic: {topic}{Environment.NewLine}" +
                $"message:{message}{Environment.NewLine}" +
                $"user: {_userId}{Environment.NewLine}" +
                $"company: {_companyId}{Environment.NewLine}" +
                $"{Environment.NewLine}";
            Console.WriteLine(line);
        }

        public void Log(
            IEnumerable<string> messages,
            string topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            var message = string.Empty;
            foreach (var msg in messages)
            {
                if (!string.IsNullOrEmpty(message))
                    message += Environment.NewLine;
                message += msg;
            }
            Log(message, topic, severity, memberName, filePath, lineNumber);
        }

        public void Log(
            IMethodResult result,
            string topic,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            Log(result.ErrorMessage, topic, result.Severity, memberName, filePath, lineNumber);
        }

        public void Log(
            Exception e,
            string topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            var message = e.GetDeepMessage();
            Log(message, topic, severity, memberName, filePath, lineNumber);
        }

    }
}
