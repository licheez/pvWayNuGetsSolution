using Npgsql;
using PvWayDaoAbstractions;
using System.Data;

namespace PvWayPgSqlDao;

internal class PgSqlStoredProcExecutor :
    PgSqlCommandExecutor, IDaoStoredProcExecutor
{
    public PgSqlStoredProcExecutor(
        Func<Exception, Task> logAsync,
        NpgsqlConnection cn) : base(logAsync, cn)
    {
    }

    public override IDbCommand CreateCommand(
        string commandText, IDbTransaction? transaction = null)
    {
        var cmd = Cn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = commandText;
        if (transaction != null) cmd.Transaction =
            (NpgsqlTransaction)transaction;
        return cmd;
    }

    public void AddStringParam(
        IDbCommand cmd, string paramName, string paramValue)
    {
        var param = new NpgsqlParameter
        {
            DbType = DbType.String,
            ParameterName = paramName,
            Value = paramValue
        };
        cmd.Parameters.Add(param);
    }
}