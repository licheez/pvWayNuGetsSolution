using System;
using System.Threading.Tasks;

namespace pvWay.Crypto
{
    public interface ICrypto: IAsyncDisposable, IDisposable
    {
        Task<string> EncryptAsync(string text);
        Task<string> EncryptAsync<T>(T data) where T: class;

        Task<string> DecryptAsync(string base64String);
        Task<T> DecryptAsync<T>(string base64String) where T: class;
    }
}