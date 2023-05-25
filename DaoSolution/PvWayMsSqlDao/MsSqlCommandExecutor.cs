using PvWayDaoAbstractions;
using System.Data;
using System.Data.SqlClient;

namespace PvWayMsSqlDao;

internal abstract class MsSqlCommandExecutor : IDaoCommandExecutor
{
    private readonly Func<Exception, Task> _logAsync;
    protected readonly SqlConnection Cn;

    protected MsSqlCommandExecutor(
        Func<Exception, Task> logAsync,
        SqlConnection cn)
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
            var sqlCmd = (SqlCommand)cmd;
            var reader = await sqlCmd.ExecuteReaderAsync();
            var dr = new MsSqlDaoReader(reader);
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
            var sqlCmd = (SqlCommand)cmd;
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
        var sqlCmd = (SqlCommand)cmd;

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