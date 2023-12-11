using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

internal sealed class MsSqlLogWriter : IMsSqlLogWriter
{
    private readonly IConnectionStringProvider _csp;
    
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

    public MsSqlLogWriter(
        IConnectionStringProvider csp,
        IMsSqlLogWriterConfig config)
    {
        _csp = csp;
        _schemaName = config.SchemaName;
        _tableName = config.TableName;
        _userIdColumnName = config.UserIdColumnName;
        _companyIdColumnName = config.CompanyIdColumnName;
        _machineNameColumnName = config.MachineNameColumnName;
        _severityCodeColumnName = config.SeverityCodeColumnName;
        _contextColumnName = config.ContextColumnName;
        _topicColumnName = config.TopicColumnName;
        _messageColumnName = config.MessageColumnName;
        _createDateColumnName = config.CreateDateUtcColumnName;

        CheckTable();
    }
    
    public void WriteLog(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        WriteLogAsync(
            userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc).Wait();
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        SeverityEnu severity, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        var cmdText = GetCommandText(
            userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc);
        var cs = await _csp.GetConnectionStringAsync();
        await using var cn = new SqlConnection(cs);
        await cn.OpenAsync();
        var cmd = new SqlCommand(cmdText, cn)
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
        SeverityEnu severity, string machineName,
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
        var severityCode = EnumSeverity.GetCode(severity);
        string pSeverityCode;
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

        var cmdText = $"INSERT INTO [{_schemaName}].[{_tableName}] "
                      + "( "
                      + $" [{_userIdColumnName}], "
                      + $" [{_companyIdColumnName}], "
                      + $" [{_severityCodeColumnName}], "
                      + $" [{_machineNameColumnName}], "
                      + $" [{_topicColumnName}], "
                      + $" [{_contextColumnName}], "
                      + $" [{_messageColumnName}], "
                      + $" [{_createDateColumnName}] "
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

    private static string TruncateThenEscape(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var val = value.Length > maxLength
            ? value[..(maxLength - 3)] + "..."
            : value;
        return val.Replace("'", "''");
    }

    private void CheckTable()
    {
        Console.WriteLine($"checking {_schemaName}.{_tableName}");
        var cs = _csp.GetConnectionStringAsync().Result;
        using var cn = new SqlConnection(cs);

        cn.Open();
        var cmdText = "SELECT [column_name], " +
                      "       [data_type], " +
                      "       [is_nullable], " +
                      "       [character_maximum_length] "
                      + "FROM [information_schema].[columns] "
                      + $"WHERE [table_schema] = '{_schemaName}' " +
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
            CheckColumn(errors, dic, _topicColumnName, "varchar",
                true, out _topicLength);
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

        if (!errors.Any()) return;

        var sb = new StringBuilder();
        foreach (var error in errors)
        {
            if (sb.Length > 0)
                sb.Append(Environment.NewLine);
            sb.Append(error);
        }

        var errorMessage = sb.ToString();
        throw new MsSqlLogWriterException(errorMessage);
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

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}