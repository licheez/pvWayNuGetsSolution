using System.Runtime.CompilerServices;
using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6
{
    public abstract class Logger : ILoggerService
    {
        private readonly ILogWriter _logWriter;
        private string? _userId;
        private string? _companyId;
        private string? _topic;

        protected Logger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

#pragma warning disable CA1816
        public void Dispose()
#pragma warning restore CA1816
        {
            _logWriter.Dispose();
        }

        public void SetUser(string? userId, string? companyId = null)
        {
            _userId = userId;
            _companyId = companyId;
        }

        public void SetTopic(string? topic)
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
            WriteLog(message, _topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            string message,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(message, _topic, severity,
                memberName, filePath, lineNumber);
        }

        public void Log(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            WriteLog(GetMessage(messages), _topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(GetMessage(messages), _topic, severity,
                memberName, filePath, lineNumber);
        }

        public void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            WriteLog(e.GetDeepMessage(), _topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(e.GetDeepMessage(), _topic, severity,
                memberName, filePath, lineNumber);
        }

        public void Log(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            WriteLog(message, topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(message, topic, severity,
                memberName, filePath, lineNumber);
        }

        public void Log(
            IEnumerable<string> messages,
            string? topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            WriteLog(GetMessage(messages), topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            IEnumerable<string> messages,
            string? topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(GetMessage(messages), topic, severity,
                memberName, filePath, lineNumber);
        }

        public void Log(
            Exception e,
            string? topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            WriteLog(e.GetDeepMessage(), topic, severity,
                memberName, filePath, lineNumber);
        }

        public async Task LogAsync(
            Exception e,
            string? topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            await WriteLogAsync(e.GetDeepMessage(), topic, severity,
                memberName, filePath, lineNumber);
        }

        private static string GetMessage(IEnumerable<string> messages)
        {
            var message = string.Empty;
            foreach (var msg in messages)
            {
                if (!string.IsNullOrEmpty(message))
                    message += Environment.NewLine;
                message += msg;
            }

            return message;
        }

        private void WriteLog(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            string memberName = "",
            string filePath = "",
            int lineNumber = -1)
        {
            _logWriter.WriteLog(
                _userId, _companyId, topic,
                severity,
                Environment.MachineName,
                memberName, filePath, lineNumber,
                message, DateTime.UtcNow);
        }

        private async Task WriteLogAsync(
            string message,
            string? topic,
            SeverityEnum severity = SeverityEnum.Debug,
            string memberName = "",
            string filePath = "",
            int lineNumber = -1)
        {
            await _logWriter.WriteLogAsync(
                _userId, _companyId, topic,
                severity,
                Environment.MachineName,
                memberName, filePath, lineNumber,
                message, DateTime.UtcNow);
        }

#pragma warning disable CA1816
        public ValueTask DisposeAsync()
#pragma warning restore CA1816
        {
            return _logWriter.DisposeAsync();
        }
    }
}
