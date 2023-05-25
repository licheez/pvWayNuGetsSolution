using System.Data;

namespace PvWayDaoAbstractions;

public interface IDaoTextCommandExecutor : IDaoCommandExecutor
{
    Task<IEnumerable<T>> ExecuteReaderAsync<T>(
        Func<IDaoReader, T> factor,
        IDbCommand cmd);

    Task ExecuteNonQueryAsync(IDbCommand cmd);
}