using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PvWay.Crypto.nc8;

/// <summary>
/// Dependency injector
/// </summary>
public static class PvWayCryptoDi
{
    /// <summary>
    /// Factors the Crypto service. Provide a 32 char key and a 16 char iv
    /// </summary>
    /// <param name="key">should be exactly 32 characters long</param>
    /// <param name="initializationVector">should be exactly 16 characters long</param>
    /// <param name="defaultValidity">default validity for ephemeral encryption</param>
    public static ICrypto Create(
        string key, string initializationVector,
        TimeSpan defaultValidity)
    {
        return new Crypto(key, initializationVector, defaultValidity);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services">IServiceCollection (this is an extension method)</param>
    /// <param name="config">
    /// Should contain the following keys: Key, InitializationVector, DefaultValidity, and ValidityUnit.
    /// ValidityUnit can be 'second', 'minute', 'hour' or 'day' (plural words are supported as well)
    /// </param>
    /// <param name="lifetime">By default this will be Transient</param>
    /// <exception cref="PvWayCryptoException"></exception>
    public static void AddPvWayCrypto(
        this IServiceCollection services,
        IConfiguration config,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var key = config["Key"]!;
        var initializationVector = config["InitializationVector"]!;

        var validityStr = config["DefaultValidity"]!;
        var n = Convert.ToInt32(validityStr);
        var validityUnit = config["ValidityUnit"]!.ToLower();
        TimeSpan defaultValidity;
        switch (validityUnit)
        {
            case "second":
            case "seconds":
                defaultValidity = TimeSpan.FromSeconds(n);
                break;
            case "minute":
            case "minutes":
                defaultValidity = TimeSpan.FromMinutes(n);
                break;
            case "hour":
            case "hours":
                defaultValidity = TimeSpan.FromHours(n);
                break;
            case "day":
            case "days":
                defaultValidity = TimeSpan.FromDays(n);
                break;
            default:
                throw new PvWayCryptoException("Invalid validity unit");
        }
        
        var sd = new ServiceDescriptor(
            typeof(ICrypto),
            _ => new Crypto(
                key, initializationVector, defaultValidity), lifetime);
        
        services.TryAdd(sd);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services">this is an extension method</param>
    /// <param name="key">should be exactly 32 characters long</param>
    /// <param name="initializationVector">should be exactly 16 characters long</param>
    /// <param name="defaultValidity">default validity for ephemeral encryption</param>
    /// <param name="lifetime">Defaults to Transient</param>
    public static void AddPvWayCrypto(
        this IServiceCollection services,
        string key, string initializationVector,
        TimeSpan defaultValidity,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var sd = new ServiceDescriptor(
            typeof(ICrypto),
            _ => new Crypto(
                key, initializationVector, defaultValidity), lifetime);
        services.TryAdd(sd);
    }
    
}