namespace PvWay.LoggerService.Abstractions.nc8;

public interface IConnectionStringProvider
{
    Task<string> GetConnectionStringAsync(
        SqlRoleEnu role = SqlRoleEnu.Application);
}