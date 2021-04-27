using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace pvWay.OracleLogWriter.Fw
{
    public class OracleLogWriterSingleton : IOracleLogWriter
    {
        private static volatile IOracleLogWriter _instance;
        private static readonly object Locker = new object();

        private readonly string _connectionString;

        private readonly string _tableName;

        private readonly string _userIdColumnName;
        private int _userIdLength;

        private readonly string _companyIdColumnName;
        private int _companyIdLength;

        private readonly string _topicColumnName;
        private int _topicLength;

        private readonly string _machineNameColumnName;
        private int _machineNameLength;

        private readonly string _severityCodeColumnName;

        private readonly string _contextColumnName;
        private int _contextLength;

        private readonly string _messageColumnName;
        private int _messageLength;

        private readonly string _createDateColumnName;

        private OracleLogWriterSingleton(
            string connectionString,
            string tableName = "Log",
            string userIdColumnName = "UserId",
            string companyIdColumnName = "CompanyId",
            string machineNameColumnName = "MachineName",
            string severityCodeColumnName = "SeverityCode",
            string contextColumnName = "Context",
            string topicColumnName = "Topic",
            string messageColumnName = "Message",
            string createDateColumnName = "CreateDateUtc")
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _userIdColumnName = userIdColumnName;
            _companyIdColumnName = companyIdColumnName;
            _machineNameColumnName = machineNameColumnName;
            _severityCodeColumnName = severityCodeColumnName;
            _contextColumnName = contextColumnName;
            _topicColumnName = topicColumnName;
            _messageColumnName = messageColumnName;
            _createDateColumnName = createDateColumnName;

            CheckTable();
        }

        public static IOracleLogWriter GetInstance(
            string connectionString,
            string tableName = "Log",
            string userIdColumnName = "UserId",
            string companyIdColumnName = "CompanyId",
            string machineNameColumnName = "MachineName",
            string severityCodeColumnName = "SeverityCode",
            string contextColumnName = "Context",
            string topicColumnName = "Topic",
            string messageColumnName = "Message",
            string createDateColumnName = "CreateDateUtc")
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                return _instance ?? (_instance = new OracleLogWriterSingleton(
                    connectionString,
                    tableName,
                    userIdColumnName,
                    companyIdColumnName,
                    machineNameColumnName,
                    severityCodeColumnName,
                    contextColumnName,
                    topicColumnName,
                    messageColumnName,
                    createDateColumnName));
            }
        }

        public void Dispose()
        {
            // nop
        }

        public void WriteLog(
            string userId, string companyId, 
            string topic, string severityCode, 
            string machineName, string memberName,
            string filePath, int lineNumber, 
            string message, DateTime dateUtc)
        {
            var cmdText = GetCommandText(
                userId, companyId, topic,
                severityCode, machineName,
                memberName, filePath, lineNumber,
                message, dateUtc);

            using (var cn = new OracleConnection(_connectionString))
            {
                cn.Open();

                var cmd = cn.CreateCommand();
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public async Task WriteLogAsync(
            string userId, string companyId, 
            string topic, string severityCode, 
            string machineName, string memberName, 
            string filePath, int lineNumber, 
            string message, DateTime dateUtc)
        {
            var cmdText = GetCommandText(
                userId, companyId, topic,
                severityCode, machineName,
                memberName, filePath, lineNumber,
                message, dateUtc);

            using (var cn = new OracleConnection(_connectionString))
            {
                cn.Open();

                var cmd = cn.CreateCommand();
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private string GetCommandText(
            string userId, string companyId, string topic,
            string severityCode, string machineName,
            string memberName, string filePath, int lineNumber,
            string message, DateTime dateUtc)
        {
            // userId
            string pUserId;
            if (string.IsNullOrEmpty(userId))
            {
                pUserId = "NULL";
            }
            else
            {
                userId = TruncateWhenToLong(userId, _userIdLength);
                userId = userId.Replace("'", "''");
                pUserId = $"'{userId}'";
            }

            // companyId
            string pCompanyId;
            if (string.IsNullOrEmpty(companyId))
            {
                pCompanyId = "NULL";
            }
            else
            {
                companyId = TruncateWhenToLong(companyId, _companyIdLength);
                companyId = companyId.Replace("'", "''");
                pCompanyId = $"'{companyId}'";
            }

            // topic
            string pTopic;
            if (string.IsNullOrEmpty(topic))
            {
                pTopic = "NULL";
            }
            else
            {
                topic = TruncateWhenToLong(topic, _topicLength);
                topic = topic.Replace("'", "''");
                pTopic = $"'{topic}'";
            }

            // severityCode
            string pSeverityCode;
            if (string.IsNullOrEmpty(severityCode))
            {
                pSeverityCode = "'D'";
            }
            else
            {
                if (severityCode.Length > 1)
                    severityCode = severityCode.Substring(0, 1);
                severityCode = severityCode.Replace("'", "''");
                pSeverityCode = $"'{severityCode}'";
            }

            // machineName
            if (string.IsNullOrEmpty(machineName))
            {
                machineName = Environment.MachineName;
            }
            machineName = TruncateWhenToLong(machineName, _machineNameLength);
            machineName = machineName.Replace("'", "''");
            var pMachineName = $"'{machineName}'";

            // memberName, filePath & lineNumber -> context
            var context = $"{memberName} # {filePath} # {lineNumber}";
            context = context.Replace("'", "''");
            context = TruncateWhenToLong(context, _contextLength);
            var pContext = $"'{context}'";

            // message
            message = message?.Replace("'", "''") ?? "na";
            message = TruncateWhenToLong(message, _messageLength);
            var pMessage = $"'{message}'";

            // date
            var locDate = dateUtc.ToLocalTime();
            var pDate = $"TO_TIMESTAMP('{locDate:yyyy-MM-dd HH:mm:ss.sss}', 'YYYY-MM-DD HH24:MI:SS.FF')";

            var cmdText = $"INSERT INTO {_tableName} "
                          + "( "
                          + $" {_userIdColumnName}, "
                          + $" {_companyIdColumnName}, "
                          + $" {_severityCodeColumnName}, "
                          + $" {_machineNameColumnName}, "
                          + $" {_topicColumnName}, "
                          + $" {_contextColumnName}, "
                          + $" {_messageColumnName}, "
                          + $" {_createDateColumnName} "
                          + ")"
                          + "VALUES "
                          + "( "
                          + $"{pUserId}, "
                          + $"{pCompanyId}, "
                          + $"{pSeverityCode}, "
                          + $"{pMachineName}, "
                          + $"{pTopic}, "
                          + $"{pContext}, "
                          + $"{pMessage}, "
                          + $"{pDate}"
                          + ")";
            return cmdText;
        }

        private static string TruncateWhenToLong(string value, int maxLength)
        {
            return value.Length > maxLength
                ? value.Substring(0, maxLength - 3) + "..."
                : value;
        }

        private void CheckTable()
        {
            var dic = new Dictionary<string, ColumnInfo>();

            using (var cn = new OracleConnection(_connectionString))
            {
                cn.Open();

                var cmdText =
                    "SELECT COLUMN_NAME, "
                    + "     DATA_TYPE, "
                    + "     DATA_LENGTH, "
                    + "     NULLABLE "
                    + "FROM ALL_TAB_COLS "
                    + $"WHERE TABLE_NAME = '{_tableName}'";

                var cmd = cn.CreateCommand();
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ci = new ColumnInfo(reader);
                    dic.Add(ci.ColumnName, ci);
                }
                reader.Close();
                cn.Close();
            }

            var errors = new List<string>();
            if (dic.Count == 0)
            {
                errors.Add($"table {_tableName} not found");
            }
            else
            {
                CheckColumn(errors, dic, _userIdColumnName, "VARCHAR",
                    true, out _userIdLength);
                CheckColumn(errors, dic, _companyIdColumnName, "VARCHAR",
                    true, out _companyIdLength);
                CheckColumn(errors, dic, _severityCodeColumnName, "CHAR",
                    false, out _);
                CheckColumn(errors, dic, _machineNameColumnName, "VARCHAR",
                    false, out _machineNameLength);
                CheckColumn(errors, dic, _topicColumnName, "VARCHAR",
                    true, out _topicLength);
                CheckColumn(errors, dic, _contextColumnName, "VARCHAR",
                    false, out _contextLength);
                CheckColumn(errors, dic, _messageColumnName, "VARCHAR",
                    false, out _messageLength);
                CheckColumn(errors, dic, _createDateColumnName, "TIMESTAMP",
                    false, out _);
            }


            if (!errors.Any()) return;
            
            var errorMessage = string.Empty;
            foreach (var error in errors)
            {
                if (!string.IsNullOrEmpty(errorMessage)) errorMessage += Environment.NewLine;
                errorMessage += error;
            }
            throw new Exception(errorMessage);

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

            var type = info.Type.ToUpper();
            expectedType = expectedType.ToUpper();
            if (!type.StartsWith(expectedType))
            {
                errors.Add($"{columnName} expected type is {expectedType} but actual type is {type}");
                return;
            }

            length = info.Length ?? 0;

            if (isNullable == info.IsNullable) return;

            var neg = isNullable ? "" : "not ";
            errors.Add($"{columnName} should {neg}be nullable");
        }

    }
}