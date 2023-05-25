using System.Data;

namespace PvWayDaoAbstractions;

public interface IDaoCommandExecutor
{
    IDbCommand CreateCommand(string commandText,
        IDbTransaction? transaction = null);
    Task<object?> ExecuteScalarAsync(IDbCommand cmd);
}