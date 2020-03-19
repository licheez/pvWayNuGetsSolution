﻿using pvWay.MethodResultWrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace pvWay.MsSqlLoggerService
{
    internal class ColumnInfo
    {
        public string ColumnName { get; }
        public string Type { get; }
        public int? Length { get; }
        public bool IsNullable { get; }

        public ColumnInfo(IDataRecord dr)
        {
            var iName = dr.GetOrdinal("column_name");
            ColumnName = dr.GetString(iName);

            var iDataType = dr.GetOrdinal("data_type");
            Type = dr.GetString(iDataType);

            var iLength = dr.GetOrdinal("character_maximum_length");

            Length = dr.IsDBNull(iLength)
                ? (int?)null : dr.GetInt32(iLength);

            var iIsNullable = dr.GetOrdinal("is_nullable");
            IsNullable = dr.GetString(iIsNullable) == "YES";
        }
    }

    public class Logger : ILoggerService
    {
        //private static volatile ILoggerService _instance;
        //private static readonly object Locker = new object();

        private readonly string _msSqlConnectionString;
        private readonly string _tableSchema;
        private readonly string _tableName;

        private readonly SeverityEnum _logLevel;

        private readonly string _userIdColumnName;
        private int _userIdLength;

        private readonly string _companyIdColumnName;
        private int _companyIdLength;

        private readonly string _machineNameColumnName;
        private int _machineNameLength;

        private readonly string _severityCodeColumnName;

        private readonly string _contextColumnName;
        private int _contextLength;

        private readonly string _messageColumnName;

        private readonly string _createDateColumnName;

        private string _userId;
        private string _companyId;

        /// <summary>
        /// Instantiates a new LoggerService.
        /// This will check that the table exits and that all columns are compliant.
        /// The constructor also store the max length of the columns so that the
        /// info will be properly truncated when logging the data into the db.
        /// The constructor throws a system exception when the table definition
        /// does not comply to the requested pre-conditions.
        /// </summary>
        /// <param name="msSqlConnectionString"></param>
        /// <param name="logLevel">default to SeverityEnum.Debug</param>
        /// <param name="tableSchema">default to "dbo"</param>
        /// <param name="tableName">default to "ApplicationLog"</param>
        /// <param name="userIdColumnName">name of the UserId column (should be varchar nullable). default "UserId"</param>
        /// <param name="companyIdColumnName">name of the CompanyId column (should be varchar nullable). default "CompanyId"</param>
        /// <param name="machineNameColumnName">name of the MachineName column (should be varchar non nullable). default "MachineName"</param>
        /// <param name="severityCodeColumnName">name of the SeverityCode column (should be char non nullable). default "SeverityCode"</param>
        /// <param name="contextColumnName">name of the Context column (should be varchar non nullable). default "Context"</param>
        /// <param name="messageColumnName">name of the Message column (should be varchar(MAX) non nullable). default "Message"</param>
        /// <param name="createDateColumnName">name of the CreateDateUtc column (should be datetime non nullable). default "CreateDateUtc"</param>
        /// <param name="userId">Identification of the connected user (optional... can also be set later)</param>
        /// <param name="companyId">Identification of the company of the connected user (optional... can also be set later)</param>
        public Logger(
            string msSqlConnectionString,
            SeverityEnum logLevel = SeverityEnum.Debug,
            string tableSchema = "dbo",
            string tableName = "ApplicationLog",
            string userIdColumnName = "AspNetUserId",
            string companyIdColumnName = "CompanyId",
            string machineNameColumnName = "MachineName",
            string severityCodeColumnName = "SeverityCode",
            string contextColumnName = "Context",
            string messageColumnName = "Message",
            string createDateColumnName = "CreateDateUtc",
            string userId = null,
            string companyId = null)
        {
            _msSqlConnectionString = msSqlConnectionString;
            _logLevel = logLevel;
            _tableSchema = tableSchema;
            _tableName = tableName;
            _userIdColumnName = userIdColumnName;
            _companyIdColumnName = companyIdColumnName;
            _machineNameColumnName = machineNameColumnName;
            _severityCodeColumnName = severityCodeColumnName;
            _contextColumnName = contextColumnName;
            _messageColumnName = messageColumnName;
            _createDateColumnName = createDateColumnName;
            _userId = userId;
            _companyId = companyId;
            CheckTable();
        }

        private void CheckTable()
        {
            using (var cn = new SqlConnection(_msSqlConnectionString))
            {
                cn.Open();
                var cmdText = "SELECT [column_name], " +
                              "       [data_type], " +
                              "       [is_nullable], " +
                              "       [character_maximum_length] "
                            + "FROM [information_schema].[columns] "
                            + $"WHERE [table_schema] = '{_tableSchema}' " +
                              $"AND   [table_name] = '{_tableName}'";

                var cmd = cn.CreateCommand();
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                var dic = new Dictionary<string, ColumnInfo>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ci = new ColumnInfo(reader);
                    dic.Add(ci.ColumnName, ci);
                }
                reader.Close();
                cn.Close();

                var errors = new List<string>();
                if (dic.Count == 0)
                {
                    errors.Add($"table {_tableName} not found");
                }
                else
                {
                    CheckColumn(errors, dic, _userIdColumnName, "varchar", 
                        true, out _userIdLength);
                    CheckColumn(errors, dic, _companyIdColumnName, "varchar", 
                        true, out _companyIdLength);
                    CheckColumn(errors, dic, _severityCodeColumnName, "char",
                        false, out _);
                    CheckColumn(errors, dic, _machineNameColumnName, "varchar", 
                        false, out _machineNameLength);
                    CheckColumn(errors, dic, _contextColumnName, "varchar", 
                        false, out _contextLength);

                    CheckColumn(errors, dic, _messageColumnName, "nvarchar", 
                        false, out var len);
                    if (len != -1)
                    {
                        errors.Add($"column {_messageColumnName} should be nvarchar(MAX)");
                    }

                    CheckColumn(errors, dic, _createDateColumnName, "datetime",
                        false, out _);
                }

                if (errors.Any())
                {
                    var errorMessage = string.Empty;
                    foreach (var error in errors)
                    {
                        if (!string.IsNullOrEmpty(errorMessage)) errorMessage += Environment.NewLine;
                        errorMessage += error;
                    }
                    throw new Exception(errorMessage);
                }
            }
        }

        private static void CheckColumn(
            ICollection<string> errors,
            IDictionary<string, ColumnInfo> dic,
            string columnName, 
            string expectedType,
            bool isNullable,
            out int length)
        {
            length = 0;
            if (!dic.ContainsKey(columnName))
            {
                errors.Add($"{columnName} not found in log table");
                return;
            }
            var info = dic[columnName];

            var type = info.Type.ToLower();
            expectedType = expectedType?.ToLower();
            if (type != expectedType)
            {
                errors.Add($"{columnName} expected type is {expectedType} but actual type is {type}");
                return;
            }

            length = info.Length ?? 0;

            if (isNullable == info.IsNullable) return;

            var neg = isNullable ? "" : "not ";
            errors.Add($"{columnName} should {neg}be nullable");
        }
        
        public void SetUser(string userId, string companyId = null)
        {
            _userId = userId;
            _companyId = companyId;
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
            var message = e.GetDeepMessage() + $"stackTrace:{e.StackTrace}";
            Log(message, severity, callerMemberName, callerFilePath, callerLineNumber);
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
                if (context.Length > _contextLength)
                    context = context.Substring(0, _contextLength);

                message = message?.Replace("'", "''") ?? "na";

                // userId
                string uId;
                if (string.IsNullOrEmpty(_userId))
                {
                    uId = "NULL";
                }
                else
                {
                    if (_userId.Length > _userIdLength)
                        _userId = _userId.Substring(0, _userIdLength);
                    _userId = _userId.Replace("'", "''");
                    uId = $"'{_userId}'";
                }

                // companyId
                string cId;
                if (string.IsNullOrEmpty(_companyId))
                {
                    cId = "NULL";
                }
                else
                {
                    if (_companyId.Length > _companyIdLength)
                        _companyId = _companyId.Substring(0, _companyIdLength);
                    _companyId = _companyId.Replace("'", "''");
                    cId = $"'{_companyId}'";
                }

                var machineName = Environment.MachineName;
                if (machineName.Length > _machineNameLength)
                    machineName = machineName.Substring(0, _machineNameLength);
                machineName = machineName.Replace("'", "''");

                var cmdText = $"INSERT INTO [{_tableSchema}].[{_tableName}] "
                              + "( "
                              + $" [{_userIdColumnName}], "
                              + $" [{_companyIdColumnName}], "
                              + $" [{_severityCodeColumnName}], "
                              + $" [{_machineNameColumnName}], "
                              + $" [{_contextColumnName}], "
                              + $" [{_messageColumnName}], "
                              + $" [{_createDateColumnName}] "
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