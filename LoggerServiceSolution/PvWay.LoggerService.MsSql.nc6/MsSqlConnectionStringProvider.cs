using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.MsSql.nc6;

public class MsSqlConnectionStringProvider : IConnectionStringProvider
{
    private readonly Func<SqlRoleEnu, Task<string>> _getCs;

    public MsSqlConnectionStringProvider(Func<SqlRoleEnu, Task<string>> getCs)
    {
        _getCs = getCs;
    }

    public Task<string> GetConnectionStringAsync(
        SqlRoleEnu role = SqlRoleEnu.Application)
    {
        return _getCs(role);
    }
}