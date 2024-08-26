using System.Data;
using Npgsql;
using PvWay.PgSqlSemaphoreService.nc8.Exceptions;
using PvWay.PgSqlSemaphoreService.nc8.Helpers;
using PvWay.PgSqlSemaphoreService.nc8.Model;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.PgSqlSemaphoreService.nc8.Services;

internal class SemaphoreService(ISemaphoreServiceConfig config) : ISemaphoreService
{
    private readonly string _schemaName = config.SchemaName;
    private readonly string _tableName = config.TableName;
    private readonly Func<Task<string>> _getCsAsync = config.GetCsAsync;
    private readonly Action<Exception> _logException = config.LogException;
    private readonly Action<string>? _logInfo = config.LogInfo;
    
    private const string NameField = "Name";
    private const string OwnerField = "Owner";
    private const string TimeoutInSecondsField = "TimeOutInSeconds";
    private const string CreateDateUtcField = "CreateDateUtc";
    private const string UpdateDateUtcField = "UpdateDateUtc";

    public async Task<ISemaphoreInfo> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);

        var cs = await _getCsAsync();
        await using var cn = new NpgsqlConnection(cs);

        try
        {
            await cn.OpenAsync();
            
            await CreateTableIfNotExistsAsync(cn);
            
            owner = DaoHelper.TruncateThenEscape(owner, 128);
            LogInfo($"{owner} is acquiring {name}");

            var utcNow = DateTime.UtcNow;
            var sqlNow = DaoHelper.GetTimestamp(utcNow);
            var timeoutInSeconds = Convert.ToInt32(timeout.TotalSeconds);
            var insertText =
                $"INSERT INTO \"{_schemaName}\".\"{_tableName}\" " +
                "(" +
                $" \"{NameField}\", " +
                $" \"{OwnerField}\", " +
                $" \"{TimeoutInSecondsField}\", " +
                $" \"{CreateDateUtcField}\", " +
                $" \"{UpdateDateUtcField}\" " +
                ") VALUES (" +
                $"'{name}', " +
                $"'{owner}', " +
                $"{timeoutInSeconds}, " +
                $"{sqlNow}, " +
                $"{sqlNow}" +
                ")";

            await using var insertCmd = cn.CreateCommand();
            insertCmd.CommandType = CommandType.Text;
            insertCmd.CommandText = insertText;

            try
            {
                await insertCmd.ExecuteNonQueryAsync();
                LogInfo($"{owner} acquired {name}");
                return new DbSemaphore(
                    SemaphoreStatusEnu.Acquired,
                    name, owner, timeout,
                    utcNow, utcNow);
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
                        // the semaphore was released (deleted) in the meantime
                        return
                            new DbSemaphore(
                                SemaphoreStatusEnu.ReleasedInTheMeanTime,
                                name, null, timeout,
                                utcNow, utcNow);
                    }

                    // the semaphore was found
                    var timeElapsed = utcNow - fSemaphore.UpdateDateUtc;
                    // if the elapsed time is less than the timeout limit
                    // consider the semaphore is still valid
                    if (timeElapsed <= timeout)
                    {
                        LogInfo($"{name} is still in use by {fSemaphore.Owner}");
                        return new DbSemaphore(
                            SemaphoreStatusEnu.OwnedSomeoneElse,
                            fSemaphore);
                    }

                    // the elapsed time is greater than the timeout limit
                    // force the release of the semaphore
                    LogInfo($"{name} force released");
                    await ReleaseSemaphoreAsync(cn, name);
                    return new DbSemaphore(
                        SemaphoreStatusEnu.ForcedReleased,
                        fSemaphore);
                }
                catch (Exception e)
                {
                    _logException(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            _logException(e);
            throw new PvWayPgSqlSemaphoreException(e);
        }
    }

    public async Task TouchSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        var cs = await _getCsAsync();
        await using var cn = new NpgsqlConnection(cs);
        var sqlNow = DaoHelper.GetTimestamp(DateTime.UtcNow);
        var updateText = $"UPDATE \"{_schemaName}\".\"{_tableName}\" " +
                         $"SET \"{UpdateDateUtcField}\" = {sqlNow} " +
                         $"WHERE \"{NameField}\" = '{name}'";

        try
        {
            await cn.OpenAsync();
            await using var updateCmd = cn.CreateCommand();
            updateCmd.CommandType = CommandType.Text;
            updateCmd.CommandText = updateText;

            await updateCmd.ExecuteNonQueryAsync();
            LogInfo($"{name} touched");
        }
        catch (Exception e)
        {
            LogInfo(updateText);
            _logException(e);
            throw new PvWayPgSqlSemaphoreException(e);
        }
    }

    public async Task ReleaseSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        var cs = await _getCsAsync();
        await using var cn = new NpgsqlConnection(cs);

        try
        {
            await cn.OpenAsync();
            await ReleaseSemaphoreAsync(cn, name);
            LogInfo($"{name} released");
        }
        catch (Exception e)
        {
            _logException(e);
            throw;
        }
    }

    public async Task<ISemaphoreInfo?> GetSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        var cs = await _getCsAsync();
        await using var cn = new NpgsqlConnection(cs);
        await cn.OpenAsync();
        await CreateTableIfNotExistsAsync(cn);

        try
        {
            return await GetSemaphoreAsync(cn, name);
        }
        catch (Exception e)
        {
            _logException(e);
            throw;
        }
    }

    public async Task<T> IsolateWorkAsync<T>(
        string semaphoreName, 
        string owner, 
        TimeSpan timeout, 
        Func<Task<T>> workAsync, 
        Action<string>? notify = null,
        int sleepBetweenAttemptsInSeconds = 15)
    {
        while (true)
        {
            var si = await AcquireSemaphoreAsync(
                semaphoreName, owner, timeout);
            if (si.Status == SemaphoreStatusEnu.Acquired)
            {
                try
                {
                    return await workAsync();
                }
                catch (Exception e)
                {
                    _logException(e);
                    throw;
                }
                finally
                {
                    await ReleaseSemaphoreAsync(semaphoreName);
                }
            }

            if (notify != null)
            {
                var notification = $"mutex {semaphoreName} is locked by {si.Owner} " +
                                   $"since {si.CreateDateUtc} UTC " +
                                   $"It will expire on {si.ExpiresAtUtc} UTC. " +
                                   $"Sleeping {sleepBetweenAttemptsInSeconds} seconds.";
                notify(notification);
            }
            Thread.Sleep(TimeSpan.FromSeconds(sleepBetweenAttemptsInSeconds));
        }
    }

    public async Task IsolateWorkAsync(
        string semaphoreName, 
        string owner, 
        TimeSpan timeout, 
        Func<Task> workAsync, 
        Action<string>? notify = null,
        int sleepBetweenAttemptsInSeconds = 15)
    {
        await IsolateWorkAsync(
            semaphoreName,
            owner,
            timeout,
            async () =>
            {
                await workAsync.Invoke();
                return 0;
            },
            notify,
            sleepBetweenAttemptsInSeconds);
    }

    private async Task<ISemaphoreInfo?> GetSemaphoreAsync(
        NpgsqlConnection cn, string name)
    {
        var selectText =
            "SELECT " +
            $" \"{OwnerField}\", " +
            $" \"{TimeoutInSecondsField}\", " +
            $" \"{CreateDateUtcField}\", " +
            $" \"{UpdateDateUtcField}\" " +
            $"FROM \"{_schemaName}\".\"{_tableName}\" " +
            $"WHERE \"{NameField}\" = '{name}'";

        await using var selectCmd = cn.CreateCommand();
        selectCmd.CommandType = CommandType.Text;
        selectCmd.CommandText = selectText;

        try
        {
            await using var reader = await selectCmd.ExecuteReaderAsync();
            var rowFound = await reader.ReadAsync();
            if (!rowFound)
            {
                await reader.CloseAsync();
                return null;
            }
            var owner = reader.GetString(0);
            var timeoutISeconds = reader.GetInt32(1);
            var timeout = TimeSpan.FromSeconds(timeoutISeconds);
            var createDateUtc = reader.GetDateTime(2);
            var updateDateUtc = reader.GetDateTime(3);
            await reader.CloseAsync();
            return new DbSemaphore(
                SemaphoreStatusEnu.Acquired,
                name, owner, timeout,
                createDateUtc, updateDateUtc);
        }
        catch (Exception e)
        {
            LogInfo(selectText);
            _logException(e);
            Console.WriteLine(e);
            throw new PvWayPgSqlSemaphoreException(e);
        }
    }

    private async Task ReleaseSemaphoreAsync(
        NpgsqlConnection cn, string name)
    {
        var deleteText = $"DELETE FROM \"{_schemaName}\".\"{_tableName}\" " +
                          $"WHERE \"{NameField}\" = '{name}'";
        await using var deleteCmd = cn.CreateCommand();
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
            throw new PvWayPgSqlSemaphoreException(e);
        }
    }

    private void LogInfo(string info)
    {
        _logInfo?.Invoke($"pvWaySemaphoreService: {info}");
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

        LogInfo("schema or table does not exists yet");
        
        // need to be db owner for this to work
        try
        {
            LogInfo($"creating schema {_schemaName} if it does not yet exists");
            var createSchemaText =
                $"CREATE SCHEMA IF NOT EXISTS \"{_schemaName}\"";
            await using var schemaCmd = cn.CreateCommand();
            schemaCmd.CommandText = createSchemaText;
            await schemaCmd.ExecuteNonQueryAsync();
            
            LogInfo($"creating table {_tableName}");
            var createCommandText =
                $"CREATE TABLE \"{_schemaName}\".\"{_tableName}\" (" +
                $" \"{NameField}\" character varying (50) PRIMARY KEY, " +
                $" \"{OwnerField}\" character varying (128) NOT NULL, " +
                $" \"{TimeoutInSecondsField}\" integer NOT NULL, " +
                $" \"{CreateDateUtcField}\" timestamptz NOT NULL, " +
                $" \"{UpdateDateUtcField}\" timestamptz NOT NULL" +
                ")";
            await using var tableCmd = cn.CreateCommand();
            tableCmd.CommandText = createCommandText;
            await tableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logException.Invoke(e);
            throw new PvWayPgSqlSemaphoreException(e);
        }
    }
}