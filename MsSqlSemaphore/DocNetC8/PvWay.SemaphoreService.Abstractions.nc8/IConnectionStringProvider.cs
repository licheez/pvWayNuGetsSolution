namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface IConnectionStringProvider
{
    Task<string> GetConnectionStringAsync(
        SqlRoleEnu role = SqlRoleEnu.Application);
}