﻿using PvWayDaoAbstractions;
using System.Data;
using System.Data.SqlClient;

namespace PvWayMsSqlDao;

internal class MsSqlDaoService : IDaoService
{
    private readonly Func<Exception, Task> _logAsync;
    private readonly SqlConnection _cn;
    private IDbTransaction? _dbTransaction;

    public MsSqlDaoService(
        Func<Exception, Task> logAsync,
        string connectionString)
    {
        _logAsync = logAsync;
        _dbTransaction = null;
        _cn = new SqlConnection(connectionString);
    }


    public string GetDatabaseName()
    {
        return _cn.Database;
    }

    public IDaoStoredProcExecutor StoredProcExecutor =>
        new MsSqlStoredProcExecutor(_logAsync, _cn);

    public IDaoTextCommandExecutor TextCommandExecutor =>
        new MsSqlTextCommandExecutor(_logAsync, _cn);

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        await _cn.OpenAsync();
        _dbTransaction = _cn.BeginTransaction();
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