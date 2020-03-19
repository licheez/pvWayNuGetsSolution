using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace pvWay.MethodResultWrapper
{
    public interface ILoggerService : IDisposable
    {
        /// <summary>
        /// Subsequent calls to the Log method will
        /// store the provided UserId and CompanyId
        /// into their corresponding columns
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        void SetUser(string userId, string companyId = null);

        void Log(
            string message = "passed",
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IMethodResult result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);
    }
}
