using System.Data;

namespace PvWayDaoAbstractions;

public interface IDaoService : IAsyncDisposable
{
    string GetDatabaseName();
    IDaoStoredProcExecutor StoredProcExecutor { get; }
    IDaoTextCommandExecutor TextCommandExecutor { get; }
    Task<IDbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}