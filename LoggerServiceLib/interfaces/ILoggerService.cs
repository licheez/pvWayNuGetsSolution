using pvWay.MethodResult.enums;
using pvWay.MethodResult.interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace pvWay.LoggerService.interfaces
{
    public interface ILoggerService : IDisposable
    {
        Guid? UserId { get; set; }
        Guid? CompanyId { get; set; }
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
