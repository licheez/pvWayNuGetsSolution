using System;
using System.Threading.Tasks;

namespace pvWay.Crypto.Core
{
    public interface ICrypto: IAsyncDisposable, IDisposable
    {
        Task<string> EncryptAsync(string text);
        Task<string> EncryptAsync<T>(T data) where T: class;
        Task<string> EncryptEphemeralAsync(string text, TimeSpan? validity = null);
        Task<string> EncryptEphemeralAsync<T>(T data, TimeSpan? validity = null) where T : class;
        
        Task<string> DecryptAsync(string base64String);
        Task<T> DecryptAsync<T>(string base64String) where T: class;
        Task<T> DecryptEphemeralAsync<T>(string b64Str);
    }
}