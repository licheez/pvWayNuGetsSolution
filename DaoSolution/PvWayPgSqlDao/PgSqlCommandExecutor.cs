using Npgsql;
using PvWayDaoAbstractions;
using System.Data;

namespace PvWayPgSqlDao;

internal abstract class PgSqlCommandExecutor : IDaoCommandExecutor
{
    private readonly Func<Exception, Task> _logAsync;
    protected readonly NpgsqlConnection Cn;

    protected PgSqlCommandExecutor(
        Func<Exception, Task> logAsync,
        NpgsqlConnection cn)
    {
        _logAsync = logAsync;
        Cn = cn;
    }

    public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(
        Func<IDaoReader, T> factor,
        IDbCommand cmd)
    {
        var needClose = false;
        if (Cn.State != ConnectionState.Open)
        {
            await Cn.OpenAsync();
            needClose = true;
        }
        try
        {
            var sqlCmd = (NpgsqlCommand)cmd;
            var reader = await sqlCmd.ExecuteReaderAsync();
            var dr = new PgSqlDaoReader(reader);
            var items = new List<T>();
            while (await reader.ReadAsync())
            {
                var item = factor(dr);
                items.Add(item);
            }

            return items;
        }
        catch (Exception e)
        {
            await _logAsync(e);
            throw;
        }
        finally
        {
            if (needClose) await Cn.CloseAsync();
        }
    }

    public async Task ExecuteNonQueryAsync(IDbCommand cmd)
    {
        var needClose = false;
        if (Cn.State != ConnectionState.Open)
        {
            await Cn.OpenAsync();
            needClose = true;
        }
        try
        {
            var sqlCmd = (NpgsqlCommand)cmd;
            await sqlCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            await _logAsync(e);
        }
        finally
        {
            if (needClose) await Cn.CloseAsync();
        }
    }
    
    public abstract IDbCommand CreateCommand(
        string commandText, IDbTransaction? transaction = null);

    public async Task<object?> ExecuteScalarAsync(IDbCommand cmd)
    {
        var sqlCmd = (NpgsqlCommand)cmd;

        var needClose = false;
        if (Cn.State != ConnectionState.Open)
        {
            await Cn.OpenAsync();
            needClose = true;
        }

        try
        {
            return await sqlCmd.ExecuteScalarAsync();
        }
        catch (Exception e)
        {
            await _logAsync(e);
            throw;
        }
        finally
        {
            if (needClose) await Cn.CloseAsync();
        }
    }
}