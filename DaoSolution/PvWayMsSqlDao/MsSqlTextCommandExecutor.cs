using PvWayDaoAbstractions;
using System.Data;
using System.Data.SqlClient;

namespace PvWayMsSqlDao;

internal class MsSqlTextCommandExecutor :
    MsSqlCommandExecutor, IDaoTextCommandExecutor
{
    public MsSqlTextCommandExecutor(
        Func<Exception, Task> logAsync,
        SqlConnection cn) : base(logAsync, cn)
    {
    }

    public override IDbCommand CreateCommand(
        string commandText,
        IDbTransaction? transaction = null)
    {
        var cmd = Cn.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = commandText;
        if (transaction != null) cmd.Transaction = (SqlTransaction)transaction;
        return cmd;
    }

}