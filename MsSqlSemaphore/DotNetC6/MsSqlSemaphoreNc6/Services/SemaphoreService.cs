using System.Data;
using System.Data.SqlClient;
using pvWay.MsSqlSemaphore.nc6.Helpers;
using pvWay.MsSqlSemaphore.nc6.Interfaces.Enums;
using pvWay.MsSqlSemaphore.nc6.Interfaces.Model;
using pvWay.MsSqlSemaphore.nc6.Interfaces.Services;
using pvWay.MsSqlSemaphore.nc6.Model;

namespace pvWay.MsSqlSemaphore.nc6.Services;

public class SemaphoreService : ISemaphoreService
{
    private readonly string _connectionString;
    private readonly string _schemaName;
    private readonly string _tableName;
    private readonly Action<Exception> _logException;
    private readonly Action<string>? _logInfo;

    public SemaphoreService(
        string connectionString,
        string schemaName,
        string tableName,
        Action<Exception> logException,
        Action<string>? logInfo = null)
    {
        _connectionString = connectionString;
        _schemaName = schemaName;
        _tableName = tableName;
        _logInfo = logInfo;
        _logException = logException;
    }

    public async Task<DbSemaphoreStatusEnum> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        await using var cn = new SqlConnection(_connectionString);

        try
        {
            owner = DaoHelper.TruncateThenEscape(owner, 128);
            LogInfo($"{owner} is acquiring {name}");

            var utcNow = DateTime.UtcNow;
            var sqlNow = DaoHelper.GetTimestamp(utcNow);
            var insertText =
                $"INSERT INTO [{_schemaName}].[{_tableName}] " +
                "(" +
                " [Name], " +
                " [Owner], " +
                " [CreateDateUtc], " +
                " [UpdateDateUtc] " +
                ") VALUES (" +
                $"'{name}', " +
                $"'{owner}', " +
                $"{sqlNow}, " +
                $"{sqlNow}" +
                ")";

            cn.Open();
            var insertCmd = cn.CreateCommand();
            insertCmd.CommandType = CommandType.Text;
            insertCmd.CommandText = insertText;

            try
            {
                await insertCmd.ExecuteNonQueryAsync();
                LogInfo($"{owner} acquired {name}");
                return DbSemaphoreStatusEnum.Acquired;
            }
            catch (Exception)
            {
                // INSERT FAILED
                try
                {
                    LogInfo($"{name} was not acquired");
                    var fSemaphore = await GetSemaphoreAsync(cn, name);
                    if (fSemaphore == null)
                    {
                        LogInfo($"{name} was released in the mean time");
                        // the semaphore was released (deleted) in the mean time
                        return DbSemaphoreStatusEnum.ReleasedInTheMeanTime;
                    }

                    // the semaphore was found
                    var timeElapsed = utcNow - fSemaphore.LastTouchUtcDate;
                    // if the elapsed time is less than the timeout limit
                    // consider the semaphore is still valid
                    if (timeElapsed <= timeout)
                    {
                        LogInfo($"{name} is still in use by {fSemaphore.Owner}");
                        return DbSemaphoreStatusEnum.OwnedSomeoneElse;
                    }

                    // the elapsed time is greater than the timeout limit
                    // force the release of the semaphore
                    LogInfo($"{name} force released");
                    await ReleaseSemaphoreAsync(cn, name);
                    return DbSemaphoreStatusEnum.ForcedReleased;
                }
                catch (Exception e)
                {
                    _logException(e);
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task TouchSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        await using var cn = new SqlConnection(_connectionString);
        var sqlNow = DaoHelper.GetTimestamp(DateTime.UtcNow);
        var updateText = $"UPDATE [{_schemaName}].[{_tableName}] " +
                         $"SET [UpdateDateUtc] = {sqlNow} " +
                         $"WHERE [Name] = '{name}'";

        try
        {
            cn.Open();
            var updateCmd = cn.CreateCommand();
            updateCmd.CommandType = CommandType.Text;
            updateCmd.CommandText = updateText;

            await updateCmd.ExecuteNonQueryAsync();
            LogInfo($"{name} touched");
        }
        catch (Exception e)
        {
            LogInfo(updateText);
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ReleaseSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        await using var cn = new SqlConnection(_connectionString);

        try
        {
            await cn.OpenAsync();
            await ReleaseSemaphoreAsync(cn, name);
            LogInfo($"{name} released");
        }
        catch (Exception e)
        {
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IDbSemaphore?> GetSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        await using var cn = new SqlConnection(_connectionString);

        try
        {
            await cn.OpenAsync();
            return await GetSemaphoreAsync(cn, name);
        }
        catch (Exception e)
        {
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<IDbSemaphore?> GetSemaphoreAsync(
        SqlConnection cn, string name)
    {
        var selectText =
            "SELECT " +
            " [Owner], " +
            " [UpdateDateUtc] " +
            $"FROM [{_schemaName}].[{_tableName}] " +
            $"WHERE [Name] = '{name}'";

        var selectCmd = cn.CreateCommand();
        selectCmd.CommandType = CommandType.Text;
        selectCmd.CommandText = selectText;

        try
        {
            var reader = await selectCmd.ExecuteReaderAsync();
            var rowFound = await reader.ReadAsync();
            if (!rowFound) return null;
            var owner = reader.GetString(0);
            var lastTouchDate = reader.GetDateTime(1);
            return new DbSemaphore(owner, lastTouchDate);
        }
        catch (Exception e)
        {
            LogInfo(selectText);
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ReleaseSemaphoreAsync(
        SqlConnection cn, string name)
    {
        var deleteText = $"DELETE [{_schemaName}].[{_tableName}] " +
                          $"WHERE [Name] = '{name}'";
        var deleteCmd = cn.CreateCommand();
        deleteCmd.CommandType = CommandType.Text;
        deleteCmd.CommandText = deleteText;
        try
        {
            await deleteCmd.ExecuteReaderAsync();
        }
        catch (Exception e)
        {
            LogInfo(deleteText);
            _logException(e);
            Console.WriteLine(e);
            throw;
        }
    }

    private void LogInfo(string info)
    {
        _logInfo?.Invoke(info);
    }
}