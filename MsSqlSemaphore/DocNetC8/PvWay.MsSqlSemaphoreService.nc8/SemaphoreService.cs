using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PvWay.MsSqlSemaphoreService.nc8.Helpers;
using PvWay.MsSqlSemaphoreService.nc8.Model;
using PvWay.SemaphoreService.Abstractions.nc8;

namespace PvWay.MsSqlSemaphoreService.nc8;

internal interface ISemaphoreServiceConfig
{
    string SchemaName { get; }
    string TableName { get; }
    Func<Task<string>> GetCsAsync { get; }
    Action<Exception> LogException { get; }
    Action<string>? LogInfo { get; } 
}

internal class SemaphoreServiceConfig(
    string schemaName,
    string tableName,
    Func<Task<string>> getCsAsync,
    Action<Exception>? logException,
    Action<string>? logInfo)
    : ISemaphoreServiceConfig
{
    public SemaphoreServiceConfig(IConfiguration config,
        Func<Task<string>> getCsAsync,
        Action<Exception>? logException = null,
        Action<string>? logInfo = null): this(
            config["schemaName"]??"dbo",
            config["tableName"]??"Semaphore",
            getCsAsync, 
            logException, logInfo)
    {
    }

    public string SchemaName { get; } = schemaName;
    public string TableName { get; } = tableName;
    public Func<Task<string>> GetCsAsync { get; } = getCsAsync;

    public Action<Exception> LogException { get; } = logException??Console.WriteLine;
    public Action<string>? LogInfo { get; } = logInfo;
}



internal class SemaphoreService(ISemaphoreServiceConfig config) : ISemaphoreService
{
    private readonly string _schemaName = config.SchemaName;
    private readonly string _tableName = config.TableName;
    private readonly Func<Task<string>> _getCsAsync = config.GetCsAsync;
    private readonly Action<Exception> _logException = config.LogException;
    private readonly Action<string>? _logInfo = config.LogInfo;

    public async Task<ISemaphoreInfo> AcquireSemaphoreAsync(
        string name, string owner, TimeSpan timeout)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);

        var cs = await _getCsAsync();
        await using var cn = new SqlConnection(cs);

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
                " [TimeoutInSeconds], " +
                " [CreateDateUtc], " +
                " [UpdateDateUtc] " +
                ") VALUES (" +
                $"'{name}', " +
                $"{timeout.TotalSeconds}, " +
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
                return new DbSemaphore(
                    SemaphoreStatusEnu.Acquired,
                    owner, timeout,
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
                        // the semaphore was released (deleted) in the mean time
                        return
                            new DbSemaphore(
                                SemaphoreStatusEnu.ReleasedInTheMeanTime,
                                null, timeout,
                                utcNow, utcNow);
                    }

                    // the semaphore was found
                    var timeElapsed = utcNow - fSemaphore.UpdateUtcDate;
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
            throw;
        }
    }

    public async Task TouchSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        var cs = await _getCsAsync();
        await using var cn = new SqlConnection(cs);
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
            throw;
        }
    }

    public async Task ReleaseSemaphoreAsync(string name)
    {
        name = DaoHelper.TruncateThenEscape(name, 50);
        var cs = await _getCsAsync();
        await using var cn = new SqlConnection(cs);

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
        await using var cn = new SqlConnection(cs);

        try
        {
            await cn.OpenAsync();
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

    private async Task<ISemaphoreInfo?> GetSemaphoreAsync(
        SqlConnection cn, string name)
    {
        var selectText =
            "SELECT " +
            " [Owner], " +
            " [TimeoutInSeconds], " +
            " [CreateDateUtc], " +
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
            var timeoutISeconds = reader.GetInt32(1);
            var timeout = TimeSpan.FromSeconds(timeoutISeconds);
            var createDateUtc = reader.GetDateTime(2);
            var updateDateUtc = reader.GetDateTime(3);
            return new DbSemaphore(
                SemaphoreStatusEnu.Acquired,
                owner, timeout,
                createDateUtc, updateDateUtc);
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