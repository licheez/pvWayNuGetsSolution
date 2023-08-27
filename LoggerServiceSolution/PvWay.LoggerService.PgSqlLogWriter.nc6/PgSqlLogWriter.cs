using System.Data;
using System.Diagnostics;
using Npgsql;
using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerService.PgSqlLogWriter.nc6;
internal class PgSqlLogWriter: IPvWayPostgreLogWriter
{
    private static volatile IPvWayPostgreLogWriter? _instance;
    private static readonly object Locker = new();

    private readonly Func<Task<string>> _getConnectionStringAsync;

    private readonly string _schemaName;
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

    private readonly string _createDateColumnName;

    private PgSqlLogWriter(
        Func<Task<string>> getConnectionStringAsync,
        string tableName,
        string schemaName,
        string userIdColumnName,
        string companyIdColumnName,
        string machineNameColumnName,
        string severityCodeColumnName,
        string contextColumnName,
        string topicColumnName,
        string messageColumnName,
        string createDateColumnName)
    {
        _getConnectionStringAsync = getConnectionStringAsync;
        _schemaName = schemaName;
        _tableName = tableName;
        _userIdColumnName = userIdColumnName;
        _companyIdColumnName = companyIdColumnName;
        _machineNameColumnName = machineNameColumnName;
        _severityCodeColumnName = severityCodeColumnName;
        _contextColumnName = contextColumnName;
        _topicColumnName = topicColumnName;
        _messageColumnName = messageColumnName;
        _createDateColumnName = createDateColumnName;

        CheckTable().Wait();
    }

    /// <summary>
    /// This will check that the table is present
    /// into the provided Db and check that the table
    /// complies (column types and length).
    /// Checking the table only occurs once. i.e. The first
    /// time the FactorLoggerService is called.
    /// </summary>
    /// <param name="getConnectionStringAsync"></param>
    /// <param name="tableName"></param>
    /// <param name="schemaName"></param>
    /// <param name="userIdColumnName"></param>
    /// <param name="companyIdColumnName"></param>
    /// <param name="machineNameColumnName"></param>
    /// <param name="severityCodeColumnName"></param>
    /// <param name="contextColumnName"></param>
    /// <param name="topicColumnName"></param>
    /// <param name="messageColumnName"></param>
    /// <param name="createDateColumnName"></param>
    /// <returns>A transient ILogWriter</returns>
    public static IPvWayPostgreLogWriter FactorLogWriter(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "public",
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
            if (_instance == null)
            {
                _instance = new PgSqlLogWriter(
                    getConnectionStringAsync,
                    tableName,
                    schemaName,
                    userIdColumnName,
                    companyIdColumnName,
                    machineNameColumnName,
                    severityCodeColumnName,
                    contextColumnName,
                    topicColumnName,
                    messageColumnName,
                    createDateColumnName);
            }
        }
        return _instance;
    }

    /// <summary>
    /// This will check that the table is present
    /// into the provided Db and check that the table
    /// complies (column types and length).
    /// Checking the table only occurs once. i.e. The first
    /// time the FactorLoggerService is called.
    /// </summary>
    /// <param name="getConnectionStringAsync"></param>
    /// <param name="tableName"></param>
    /// <param name="schemaName"></param>
    /// <param name="userIdColumnName"></param>
    /// <param name="companyIdColumnName"></param>
    /// <param name="machineNameColumnName"></param>
    /// <param name="severityCodeColumnName"></param>
    /// <param name="contextColumnName"></param>
    /// <param name="topicColumnName"></param>
    /// <param name="messageColumnName"></param>
    /// <param name="createDateColumnName"></param>
    /// <returns>A transient LoggerService</returns>
    public static IPvWayPostgreLoggerService FactorLoggerService(
        Func<Task<string>> getConnectionStringAsync,
        string tableName = "AppLog",
        string schemaName = "public",
        string userIdColumnName = "UserId",
        string companyIdColumnName = "CompanyId",
        string machineNameColumnName = "MachineName",
        string severityCodeColumnName = "SeverityCode",
        string contextColumnName = "Context",
        string topicColumnName = "Topic",
        string messageColumnName = "Message",
        string createDateColumnName = "CreateDateUtc")
    {
        var lw = FactorLogWriter(
            getConnectionStringAsync,
            tableName,
            schemaName,
            userIdColumnName,
            companyIdColumnName,
            machineNameColumnName,
            severityCodeColumnName,
            contextColumnName,
            topicColumnName,
            messageColumnName,
            createDateColumnName);

        var pl = PvWayLoggerService
            .CreatePersistenceLoggerService(
            () => lw.Dispose(),
            WriteLog, WriteLogAsync);

        var ls = new PgPersistenceLogger(pl);
        return ls;

        async Task WriteLogAsync(
            (string? userId, string? companyId, string? topic,
                SeverityEnum severity, string machineName,
                string memberName, string filePath, int lineNumber,
                string message, DateTime dateUtc) log)
        {
            await lw.WriteLogAsync(
                log.userId, log.companyId, log.topic,
                log.severity, log.machineName,
                log.memberName, log.filePath, log.lineNumber,
                log.message, log.dateUtc);
        }

        void WriteLog(
            (string? userId, string? companyId, string? topic,
                SeverityEnum severity, string machineName,
                string memberName, string filePath, int lineNumber,
                string message, DateTime dateUtc) log)
        {
            lw.WriteLog(
                log.userId, log.companyId, log.topic,
                log.severity, log.machineName,
                log.memberName, log.filePath, log.lineNumber,
                log.message, log.dateUtc);
        }
    }

    private async Task CheckTable()
    {
        Console.WriteLine($"checking {_schemaName}.{_tableName}");
        var cs = await _getConnectionStringAsync();
        await using var cn = new NpgsqlConnection(cs);
        await cn.OpenAsync();

        var cmdText = "SELECT \"column_name\", " +
                      "       \"data_type\", " +
                      "       \"is_nullable\", " +
                      "       \"character_maximum_length\" "
                      + "FROM \"information_schema\".\"columns\" "
                      + $"WHERE \"table_schema\" = '{_schemaName}' " +
                      $"AND \"table_name\" = '{_tableName}'";

        var cmd = cn.CreateCommand();
        cmd.CommandText = cmdText;
        cmd.CommandType = CommandType.Text;

        var dic = new Dictionary<string, ColumnInfo>();
        var reader = await cmd.ExecuteReaderAsync();
        while ((await reader.ReadAsync()))
        {
            var ci = new ColumnInfo(reader);
            dic.Add(ci.ColumnName, ci);
        }
        await reader.CloseAsync();
        await cn.CloseAsync();

        var errors = new List<string>();
        if (dic.Count == 0)
        {
            errors.Add($"table {_tableName} not found");
        }
        else
        {
            CheckColumn(errors, dic, _userIdColumnName, "character varying",
                true, out _userIdLength);
            CheckColumn(errors, dic, _companyIdColumnName, "character varying",
                true, out _companyIdLength);
            CheckColumn(errors, dic, _severityCodeColumnName, "character",
                false, out _);
            CheckColumn(errors, dic, _machineNameColumnName, "character varying",
                false, out _machineNameLength);
            CheckColumn(errors, dic, _topicColumnName, "character varying",
                true, out _topicLength);
            CheckColumn(errors, dic, _contextColumnName, "character varying",
                false, out _contextLength);
            CheckColumn(errors, dic, _messageColumnName, "text",
                false, out var len);
            if (len != 0)
            {
                errors.Add($"column {_messageColumnName} should be text");
            }
            CheckColumn(errors, dic, _createDateColumnName, "timestamp with time zone",
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

    private static void CheckColumn(
        ICollection<string> errors,
        IDictionary<string, ColumnInfo> dic,
        string columnName,
        string? expectedType,
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

    public void WriteLog(
        string? userId, string? companyId, string? topic,
        SeverityEnum severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        WriteLogAsync(userId, companyId, topic,
            severity, machineName, memberName,
            filePath, lineNumber, message, dateUtc).Wait();
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        SeverityEnum severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        var cmdText = GetCommandText(
            userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc);

        var cs = await _getConnectionStringAsync();
        await using var cn = new NpgsqlConnection(cs);
        await cn.OpenAsync();
        var cmd = new NpgsqlCommand(cmdText, cn)
        {
            CommandType = CommandType.Text
        };

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
            await cn.CloseAsync();
        }
    }

    private string GetCommandText(
    string? userId, string? companyId, string? topic,
    SeverityEnum severity, string machineName,
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
            userId = TruncateThenEscape(userId, _userIdLength);
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
            companyId = TruncateThenEscape(companyId, _companyIdLength);
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
            topic = TruncateThenEscape(topic, _topicLength);
            pTopic = $"'{topic}'";
        }

        // severityCode
        string pSeverityCode;
        var severityCode = EnumSeverity.GetCode(severity);
        if (string.IsNullOrEmpty(severityCode))
        {
            pSeverityCode = "'D'";
        }
        else
        {
            if (severityCode.Length > 1)
                severityCode = severityCode[..1];
            pSeverityCode = $"'{severityCode}'";
        }

        // machineName
        if (string.IsNullOrEmpty(machineName))
        {
            machineName = Environment.MachineName;
        }
        machineName = TruncateThenEscape(machineName, _machineNameLength);
        var pMachineName = $"'{machineName}'";

        // memberName, filePath & lineNumber -> context
        var context = $"{memberName} # {filePath} # {lineNumber}";
        context = context.Replace("'", "''");
        context = TruncateThenEscape(context, _contextLength);
        var pContext = $"'{context}'";

        // message
        message = message.Replace("'", "''");
        var pMessage = $"'{message}'";

        // date
        var pDate = $"'{dateUtc:yyyy-MM-dd HH:mm:ss.sss}'";

        var cmdText = $"INSERT INTO {_schemaName}.\"{_tableName}\" "
                      + "( "
                      + $" \"{_userIdColumnName}\", "
                      + $" \"{_companyIdColumnName}\", "
                      + $" \"{_severityCodeColumnName}\", "
                      + $" \"{_machineNameColumnName}\", "
                      + $" \"{_topicColumnName}\", "
                      + $" \"{_contextColumnName}\", "
                      + $" \"{_messageColumnName}\", "
                      + $" \"{_createDateColumnName}\" "
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

    private static string TruncateThenEscape(
        string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var val = value.Length > maxLength
            ? value[..(maxLength - 3)] + "..."
            : value;
        return val.Replace("'", "''");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

}