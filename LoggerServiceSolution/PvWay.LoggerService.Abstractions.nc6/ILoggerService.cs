using System.Runtime.CompilerServices;

namespace PvWay.LoggerService.Abstractions.nc6
{
    public interface ILoggerService : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Subsequent calls to the Log method will
        /// store the provided UserId and CompanyId
        /// into their corresponding columns
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        void SetUser(string? userId, string? companyId = null);

        /// <summary>
        /// Topic is an optional extra column in the log
        /// that enables grouping logs. Subsequent calls to
        /// the Log method will store the provided
        /// topic into the corresponding column
        /// </summary>
        /// <param name="topic"></param>
        void SetTopic(string? topic);

        // TOPIC-LESS METHODS
        void Log(
            string message,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        Task LogAsync(
            string message,
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

        Task LogAsync(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        Task LogAsync(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);


        // TOPIC METHODS
        void Log(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        Task LogAsync(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);
        
        void Log(
            IEnumerable<string> messages,
            string? topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        Task LogAsync(
            IEnumerable<string> messages,
            string? topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            Exception e,
            string? topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        Task LogAsync(
            Exception e,
            string? topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);
    }
}
