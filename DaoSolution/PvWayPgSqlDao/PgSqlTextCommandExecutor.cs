using Npgsql;
using PvWayDaoAbstractions;
using System.Data;

namespace PvWayPgSqlDao;

internal class PgSqlTextCommandExecutor :
    PgSqlCommandExecutor, IDaoTextCommandExecutor
{
    public PgSqlTextCommandExecutor(
        Func<Exception, Task> logAsync,
        NpgsqlConnection cn) : base(logAsync, cn)
    {
    }

    public override IDbCommand CreateCommand(
        string commandText, 
        IDbTransaction? transaction = null)
    {
        var cmd = Cn.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = commandText;
        if (transaction != null) cmd.Transaction = (NpgsqlTransaction)transaction;
        return cmd;
    }

}