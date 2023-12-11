using Microsoft.Extensions.Configuration;
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MsSql.nc8;

public class ConnectionStringProvider(string cs) : IConnectionStringProvider
{
    public ConnectionStringProvider(IConfiguration? config) 
        : this(config?.GetConnectionString("logDb") ?? "localhost")
    {
    }
    
    public Task<string> GetConnectionStringAsync()
    {
        return Task.FromResult(cs);
    }
}