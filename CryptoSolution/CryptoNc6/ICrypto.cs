namespace pvWay.Crypto.nc6;

public interface ICrypto: IAsyncDisposable, IDisposable
{
    Task<string> EncryptStringAsync(string text);
    Task<string> EncryptObjectAsync<T>(T data) where T: class;
        
    Task<string> EncryptEphemeralStringAsync(
        string text, TimeSpan? validity = null);
    Task<string> EncryptEphemeralObjectAsync<T>(
        T data, TimeSpan? validity = null) 
        where T : class;
        
    Task<string> DecryptStringAsync(string base64Str);
    /// <summary>
    /// Tries to decrypt the payload
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <typeparam name="T">type of underlying object</typeparam>
    /// <returns></returns>
    Task<T> DecryptObjectAsync<T>(string base64Str) where T: class;
        
    /// <summary>
    /// Tries to decrypt the payload to primitive
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <returns>null if the payload has expired</returns>
    Task<string?> DecryptEphemeralStringAsync(string base64Str);

    /// <summary>
    /// Tries to decrypt the payload to primitive
    /// </summary>
    /// <param name="base64Str">the encrypted payload</param>
    /// <returns>null if the payload has expired</returns>
    Task<T?> DecryptEphemeralObjectAsync<T>(string base64Str) where T : class;
}