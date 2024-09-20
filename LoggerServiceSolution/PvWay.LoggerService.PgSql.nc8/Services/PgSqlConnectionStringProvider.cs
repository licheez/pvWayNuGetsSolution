using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.PgSql.nc8.Services;

public class PgSqlConnectionStringProvider(Func<SqlRoleEnu, Task<string>> getCs) :
    IConnectionStringProvider
{
    public Task<string> GetConnectionStringAsync(SqlRoleEnu role = SqlRoleEnu.Application)
    {
        return getCs(role);
    }
}