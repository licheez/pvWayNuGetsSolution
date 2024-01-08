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