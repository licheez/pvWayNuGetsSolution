using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using pvWay.MethodResultWrapper;

namespace pvWay.MsSqlLoggerService
{
    public class Logger : ILoggerService
    {
        private readonly string _msSqlConnectionString;
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }

        private readonly SeverityEnum _logLevel;

        public Logger(
            string msSqlConnectionString,
            SeverityEnum logLevel = SeverityEnum.Debug,
            Guid? userId = null,
            Guid? companyId = null)
        {
            _msSqlConnectionString = msSqlConnectionString;
            UserId = userId;
            CompanyId = companyId;
            _logLevel = logLevel;
        }

        public void Log(
            string message = "pass",
            SeverityEnum severity = SeverityEnum.Debug,
            string callerMemberName = "",
            string callerFilePath = "",
            int callerLineNumber = -1)
        {
            if (severity < _logLevel) return;
            WriteLog(severity,
                $"{callerMemberName} # {callerFilePath} # {callerLineNumber}",
                message);
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
            string callerMemberName = "",
            string callerFilePath = "",
            int callerLineNumber = -1)
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

        private void WriteLog(
            SeverityEnum severity, string context, string message)
        {
            using (var cn = new SqlConnection(_msSqlConnectionString))
            {
                cn.Open();

                context = context?.Replace("'", "''") ?? "na";
                if (context.Length > 256) context = context.Substring(0, 256);

                message = message.Replace("'", "''");

                var uId = UserId.HasValue ? $"'{UserId.Value}'" : "NULL";

                var cId = CompanyId.HasValue ? $"'{CompanyId.Value}'" : "NULL";

                var machineName = Environment.MachineName.Replace("'", "''");
                if (machineName.Length > 50) machineName = machineName.Substring(0, 50);

                var cmdText = "INSERT INTO ApplicationLog "
                              + "( "
                              + " [AspNetUserId], "
                              + " [CompanyId], "
                              + " [SeverityCode], "
                              + " [Machine], "
                              + " [Context], "
                              + " [Message], "
                              + " [CreateDateUtc] "
                              + ")"
                              + "VALUES "
                              + "( "
                              + $"{uId}, "
                              + $"{cId}, "
                              + $"'{EnumSeverity.GetCode(severity)}', "
                              + $"'{machineName}', "
                              + $"'{context}', "
                              + $"'{message}', "
                              + $"'{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.sss}' "
                              + ")";

                var cmd = new SqlCommand(cmdText, cn)
                {
                    CommandType = CommandType.Text
                };

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error : " + e.GetDeepMessage());
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public void Dispose()
        {
            // nop
        }
    }
}
