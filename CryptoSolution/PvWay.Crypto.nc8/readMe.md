# PvWay Crypto for dotNet Core 8 

Tiny utility encrypting/decrypting texts/objects to/from base64 strings

## Interfaces
### ICrypto

```csharp
namespace PvWay.Crypto.nc8;

public interface ICrypto : IAsyncDisposable, IDisposable
{
    Task<string> EncryptStringAsync(string text);
        
    Task<string> EncryptObjectAsync<T>(T data) where T : class;
        
    /// <summary>
    /// Encrypts a string to an ephemeral payload
    /// </summary>
    /// <param name="text"></param>
    /// <param name="validity">Defaulting to the default validity of the class</param>
    /// <returns>encrypted ephemeral payload</returns>
    Task<string> EncryptEphemeralStringAsync(
        string text, TimeSpan? validity = null);
        
    /// <summary>
    /// Encrypts a type T object to an ephemeral payload
    /// </summary>
    /// <param name="data"></param>
    /// <param name="validity">Defaulting to the default validity of the class</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>encrypted ephemeral payload</returns>
    Task<string> EncryptEphemeralObjectAsync<T>(
        T data, TimeSpan? validity = null)
        where T : class;

    /// <summary>
    /// Decrypts the payload as string
    /// </summary>
    /// <param name="base64Str"></param>
    /// <returns>The decrypted string</returns>
    Task<string> DecryptStringAsync(string base64Str);

    /// <summary>
    /// Decrypts the payload as an object of type T
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <typeparam name="T">type of underlying object</typeparam>
    /// <returns>The decrypted object</returns>
    Task<T> DecryptObjectAsync<T>(string base64Str) where T : class;

    /// <summary>
    /// Tries to decrypt the ephemeral payload to a string 
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <returns>The decrypted string or null if the payload has expired</returns>
    Task<string?> DecryptEphemeralStringAsync(string base64Str);

    /// <summary>
    /// Tries to decrypt the ephemeral payload to an object of type T
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <returns>The decrypted object or null if the payload has expired</returns>
    Task<T?> DecryptEphemeralObjectAsync<T>(string base64Str) where T : class;
}
```

## Provisioning

You can use the AddPvWayCrypto ServiceCollection extension for provisioning the service

There are two overloads.
* using the IConfiguration config param
* using explicit params

```csharp
...
    /// <summary>
    /// Provisions the Crypto Service 
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
    /// Provisions the Crypto Service 
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
...    

```

## Factory

There is also a static factory

```csharp
...
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
...
```

## Usage

See here after a short Console that use the nuGet

```csharp
using CryptoNc6Console;
using pvWay.Crypto.nc8;

Console.WriteLine("Hello, pvWay Crypto nc8");
Console.WriteLine();

// the length of the key string needs to be exactly 32
const string keyString = "123456789 123456789 123456789 12";

// the length of the init vector string needs to be exactly 16
const string initializationVectorString = "0123456789ABCDEF";

// Factory the crypto object
// note that we mention 10 minutes as the default validity
// time for ephemeral objects or strings
var crypto = PvWayCryptoDi.Create(
    keyString,
    initializationVectorString,
    TimeSpan.FromMinutes(10));

Console.WriteLine("Encrypting the word 'test'");
var b64 = await crypto.EncryptStringAsync("test");
Console.WriteLine($"The word 'test' was encrypted as '{b64}'");
Console.WriteLine("Retrieving the 'test' from the encrypted payload");
var text = await crypto.DecryptStringAsync(b64);
Console.WriteLine($"Got '{text}' back");
Console.WriteLine();

var mc = new MyClass
{
    TheHeader = "Header",
    TheBody = "Body",
    TheFooter = "Footer"
};
Console.WriteLine("Encrypting the object MyClass(Header, Body, Footer)");
b64 = await crypto.EncryptObjectAsync(mc);
Console.WriteLine($"The MyClass object was encrypted as {b64}");
Console.WriteLine("Retrieving the MyClass object from the payload");
var mcBack = await crypto.DecryptObjectAsync<MyClass>(b64);
Console.WriteLine($"Got MyClass back '{mcBack.TheHeader} - {mcBack.TheBody} - {mcBack.TheFooter}'");
Console.WriteLine();

Console.WriteLine("Encrypting the object MyClass(Header, Body, Footer)");
Console.WriteLine("Payload will only stay valid for 1 second");
b64 = await crypto.EncryptEphemeralObjectAsync(
     mc, TimeSpan.FromSeconds(1));
mcBack = await crypto.DecryptEphemeralObjectAsync<MyClass>(b64);
Console.WriteLine("Retrieving the MyClass object from the payload");
Console.WriteLine($"Got MyClass back '{mcBack!.TheHeader} - {mcBack.TheBody} - {mcBack.TheFooter}'");
Console.WriteLine("Ephemeral payload is still valid");
Console.WriteLine("Sleeping a while");
Thread.Sleep(1000);
Console.WriteLine("Retrieving the MyClass object from the payload");
mcBack = await crypto.DecryptEphemeralObjectAsync<MyClass>(b64);
if (mcBack != null)
    throw new PvWayCryptoException("Payload should have expired");
Console.WriteLine("The object payload has expired as expected");
Console.WriteLine();

Console.WriteLine("Encrypting the ephemeral text 'Hello butterfly' " +
                  "that will only stay alive for 1 second");
b64 = await crypto.EncryptEphemeralStringAsync("Hello butterfly", 
    TimeSpan.FromSeconds(1));
Console.WriteLine("Retrieving the ephemeral text");
text = await crypto.DecryptEphemeralStringAsync(b64);
Console.WriteLine($"The text '{text}' is still valid");
Console.WriteLine("Sleeping a while");
Thread.Sleep(1000);
text = await crypto.DecryptEphemeralStringAsync(b64);
if (text != null)
    throw new PvWayCryptoException("Payload should have expired");
Console.WriteLine("The text payload has expired as expected");
```

Happy coding
