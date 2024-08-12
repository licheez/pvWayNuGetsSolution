namespace PvWay.LoggerService.Abstractions.nc6;

public interface IConnectionStringProvider
{
    Task<string> GetConnectionStringAsync(
        SqlRoleEnu role = SqlRoleEnu.Application);
}