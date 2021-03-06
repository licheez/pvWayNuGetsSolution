﻿using System;
using System.Collections.Generic;
using pvWay.LoggerService.interfaces;
using pvWay.MethodResult.enums;
using pvWay.MethodResult.interfaces;
using static pvWay.MethodResult.extensions.ExceptionExtension;

namespace pvWay.LoggerService.components
{
    public class ConsoleLogger : ILoggerService
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }

        public void Log(
            string message = "pass",
            SeverityEnum severity = SeverityEnum.Debug,
            string callerMemberName = "",
            string callerFilePath = "",
            int callerLineNumber = -1)
        {
            var line = $"{severity}-{callerMemberName}-{callerFilePath}-{callerLineNumber}-{message}";
            Console.WriteLine(line);
        }

        public void Log(IEnumerable<string> messages, SeverityEnum severity, string memberName = "", string filePath = "",
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
            string callerMemberName = "",
            string callerFilePath = "", int callerLineNumber = -1)
        {
            Log(e.GetDeepMessage(), severity, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Log(
            IMethodResult res,
            string callerMemberName = "",
            string callerFilePath = "",
            int callerLineNumber = -1)
        {
            Log(res.ErrorMessage, res.Severity, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Dispose()
        {
            // nop
        }
    }
}
