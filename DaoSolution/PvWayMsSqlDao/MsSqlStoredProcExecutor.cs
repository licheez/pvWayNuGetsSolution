using PvWayDaoAbstractions;
using System.Data;
using System.Data.SqlClient;

namespace PvWayMsSqlDao;

internal class MsSqlStoredProcExecutor :
    MsSqlCommandExecutor, IDaoStoredProcExecutor
{

    public MsSqlStoredProcExecutor(
        Func<Exception, Task> logAsync,
        SqlConnection cn) : base(logAsync, cn)
    {
    }

    public void AddStringParam(
        IDbCommand cmd, string paramName, string paramValue)
    {
        var param = new SqlParameter
        {
            DbType = DbType.String,
            ParameterName = paramName,
            Value = paramValue
        };
        cmd.Parameters.Add(param);
    }

    public override IDbCommand CreateCommand(
        string commandText, IDbTransaction? transaction = null)
    {
        var cmd = Cn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = commandText;
        if (transaction != null) cmd.Transaction = (SqlTransaction)transaction;
        return cmd;
    }
}