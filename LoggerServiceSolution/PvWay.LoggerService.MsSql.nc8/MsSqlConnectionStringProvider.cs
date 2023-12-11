using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

public class MsSqlConnectionStringProvider(
    Func<SqlRoleEnu, Task<string>> getCs) : IConnectionStringProvider
{
    public Task<string> GetConnectionStringAsync(
        SqlRoleEnu role = SqlRoleEnu.Application)
    {
        return getCs(role);
    }
}