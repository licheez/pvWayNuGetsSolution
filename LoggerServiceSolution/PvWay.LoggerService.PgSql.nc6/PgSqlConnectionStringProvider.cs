using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.PgSql.nc6;

public class PgSqlConnectionStringProvider : 
    IConnectionStringProvider
{
    private readonly Func<SqlRoleEnu, Task<string>> _getCs;

    public PgSqlConnectionStringProvider(Func<SqlRoleEnu, Task<string>> getCs)
    {
        _getCs = getCs;
    }

    public Task<string> GetConnectionStringAsync(SqlRoleEnu role = SqlRoleEnu.Application)
    {
        return _getCs(role);
    }
}