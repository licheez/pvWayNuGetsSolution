using Npgsql;
using PvWayDaoAbstractions;
using System.Data;

namespace PvWayPgSqlDao;

internal class PgSqlDaoService : IDaoService
{
    private readonly Func<Exception, Task> _logAsync;
    private readonly NpgsqlConnection _cn;
    private IDbTransaction? _dbTransaction;

    public PgSqlDaoService(
        Func<Exception, Task> logAsync,
        string connectionString)
    {
        _logAsync = logAsync;
        _dbTransaction = null;
        _cn = new NpgsqlConnection(connectionString);
    }

    public string GetDatabaseName()
    {
        return _cn.Database;
    }

    public IDaoStoredProcExecutor StoredProcExecutor =>
        new PgSqlStoredProcExecutor(_logAsync, _cn);

    public IDaoTextCommandExecutor TextCommandExecutor =>
        new PgSqlTextCommandExecutor(_logAsync, _cn);

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        await _cn.OpenAsync();
        _dbTransaction = await _cn.BeginTransactionAsync();
        return _dbTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        _dbTransaction?.Commit();
        await _cn.CloseAsync();
        _dbTransaction = null;
    }
    public async Task RollbackTransactionAsync()
    {
        _dbTransaction?.Rollback();
        await _cn.CloseAsync();
        _dbTransaction = null;
    }

    public ValueTask DisposeAsync()
    {
        return _cn.DisposeAsync();
    }

}