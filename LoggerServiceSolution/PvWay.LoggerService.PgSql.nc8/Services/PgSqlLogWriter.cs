using System.Data;
using System.Text;
using Npgsql;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.PgSql.nc8.Exceptions;

namespace PvWay.LoggerService.PgSql.nc8.Services;

internal sealed class PgSqlLogWriter : IPgSqlLogWriter
{
    private const string SqlVarChar = "character varying";
    private const string SqlChar = "character";
    private const string SqlUtcDateTime = "timestamp without time zone";
    
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
    private int _messageLength;

    private readonly string _createDateColumnName;

    public PgSqlLogWriter(
        IConnectionStringProvider csp,
        ISqlLogWriterConfig config)
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
        
        var cs = _csp.GetConnectionStringAsync().Result;
        using var cn = new NpgsqlConnection(cs);
        cn.Open();
        CreateTableIfNotExistsAsync(cn).Wait();
        CheckTable(cn).Wait();
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
        var cmdText = GetInsertCommandText(
            userId, companyId, topic,
            severity, machineName,
            memberName, filePath, lineNumber,
            message, dateUtc);
        var cs = await _csp.GetConnectionStringAsync();
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
            Console.WriteLine(e);
        }
        finally
        {
            await cn.CloseAsync();
        }
    }
    
    public async Task<int> PurgeLogs(IDictionary<SeverityEnu, TimeSpan> retainDi){
        var cs = await _csp.GetConnectionStringAsync();
        await using var cn = new NpgsqlConnection(cs);
        await cn.OpenAsync();
        var totRows = 0;
        try
        {
            foreach (var (severity, keep) in retainDi)
            {
                var cmdText = GetPurgeCommandText(severity, keep);
                await using var cmd = new NpgsqlCommand(cmdText, cn);
                var nbRows = await cmd.ExecuteNonQueryAsync();
                totRows += nbRows;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            await cn.CloseAsync();
        }

        return totRows;
    }

    private string GetInsertCommandText(
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
        message = TruncateThenEscape(message, _messageLength);
        var pMessage = $"'{message}'";

        // date
        var pDate = $"'{dateUtc:yyyy-MM-dd HH:mm:ss.sss}'";

        var cmdText = $"INSERT INTO \"{_schemaName}\".\"{_tableName}\" "
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

    private string GetPurgeCommandText(SeverityEnu severity, TimeSpan keep)
    {
        // severityCode
        var severityCode = EnumSeverity.GetCode(severity);
        if (severityCode.Length > 1)
            severityCode = severityCode[..1];
        var pSeverityCode = $"'{severityCode}'";

        var cutoffUtc = DateTime.UtcNow - keep;
        var pDate = $"'{cutoffUtc:yyyy-MM-dd HH:mm:ss.sss}'";

        var cmdText = $"DELETE FROM \"{_schemaName}\".\"{_tableName}\" " +
                      $"WHERE  \"{_severityCodeColumnName}\" = {pSeverityCode} " +
                      $"AND \"{_createDateColumnName}\" < {pDate}";
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

    private async Task CheckTable(NpgsqlConnection cn)
    {
        var cmdText = "SELECT \"column_name\", " +
                      "       \"data_type\", " +
                      "       \"is_nullable\", " +
                      "       \"character_maximum_length\" "
                      + "FROM \"information_schema\".\"columns\" "
                      + $"WHERE table_schema = '{_schemaName}' " 
                      + $"AND table_name = '{_tableName}'";

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
            CheckColumn(errors, dic, _userIdColumnName, SqlVarChar,
                true, out _userIdLength);
            CheckColumn(errors, dic, _companyIdColumnName, SqlVarChar,
                true, out _companyIdLength);
            CheckColumn(errors, dic, _severityCodeColumnName, SqlChar,
                false, out _);
            CheckColumn(errors, dic, _machineNameColumnName, SqlVarChar,
                false, out _machineNameLength);
            CheckColumn(errors, dic, _topicColumnName, SqlVarChar,
                true, out _topicLength);
            CheckColumn(errors, dic, _contextColumnName, SqlVarChar,
                false, out _contextLength);
            CheckColumn(errors, dic, _messageColumnName, SqlVarChar,
                false, out _messageLength);
            CheckColumn(errors, dic, _createDateColumnName, SqlUtcDateTime,
                false, out _);
        }

        if (errors.Count == 0) return;

        var sb = new StringBuilder();
        foreach (var error in errors)
        {
            if (sb.Length > 0)
                sb.Append(Environment.NewLine);
            sb.Append(error);
        }

        var errorMessage = sb.ToString();
        throw new PgSqlLogWriterException(errorMessage);
    }

    private static void CheckColumn(
        List<string> errors,
        IDictionary<string, ColumnInfo> dic,
        string columnName,
        string? expectedType,
        bool isNullable,
        out int length)
    {
        length = 0;
        if (!dic.TryGetValue(columnName, out var info))
        {
            errors.Add($"{columnName} not found in log table");
            return;
        }

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

    private async Task CreateTableIfNotExistsAsync(NpgsqlConnection cn)
    {
        var existsCommandText =
            "SELECT 1 FROM information_schema.tables " +
            $"   WHERE table_schema = '{_schemaName}' " +
            $"   AND   table_name = '{_tableName}' ";

        await using var existsCmd = cn.CreateCommand();
        existsCmd.CommandText = existsCommandText;
        await using var reader = await existsCmd.ExecuteReaderAsync();
        var tableExists = await reader.ReadAsync();
        await reader.CloseAsync();

        if (tableExists) return;

        Console.WriteLine("schema or table does not exists yet");

        // need to be db owner for this to work
        try
        {
            Console.WriteLine($"creating schema {_schemaName} if it does not yet exists");
            var createSchemaText =
                $"CREATE SCHEMA IF NOT EXISTS \"{_schemaName}\"";
            await using var schemaCmd = cn.CreateCommand();
            schemaCmd.CommandText = createSchemaText;
            await schemaCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"creating table {_tableName}");
            var createCommandText =
                $"CREATE TABLE \"{_schemaName}\".\"{_tableName}\" (" +

                $" \"Id\" integer GENERATED BY DEFAULT AS IDENTITY, " +
                $" \"{_userIdColumnName}\" character varying(50) NULL, " +
                $" \"{_companyIdColumnName}\" character varying (128) NULL, " +
                $" \"{_severityCodeColumnName}\" character(1) NOT NULL, " +
                $" \"{_machineNameColumnName}\" character varying (128) NOT NULL, " +
                $" \"{_topicColumnName}\" character varying (128) NULL, " +
                $" \"{_contextColumnName}\" character varying (512) NOT NULL, " +
                $" \"{_messageColumnName}\" character varying (4096) NOT NULL, " +
                $" \"{_createDateColumnName}\" timestamp without time zone NOT NULL" +
                ")";
            await using var tableCmd = cn.CreateCommand();
            tableCmd.CommandText = createCommandText;
            await tableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new PgSqlLogWriterException(e.Message, e);
        }
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}