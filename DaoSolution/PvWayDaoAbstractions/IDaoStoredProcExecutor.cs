using System.Data;

namespace PvWayDaoAbstractions;

public interface IDaoStoredProcExecutor : IDaoTextCommandExecutor
{
    void AddStringParam(IDbCommand cmd, string paramName, string paramValue);
}