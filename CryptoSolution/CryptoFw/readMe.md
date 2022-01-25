# Crypto Fw

Tiny utilily encrypting/decrypting texts/objects to/from base64 strings

## Interfaces

This nuGet has only one public class implementing the following interface

### ICrypto

```csharp
using System;
using System.Threading.Tasks;

namespace pvWay.Crypto.Fw
{
    public interface ICrypto: IDisposable
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

```

## Usage

See here after a short Console that uses the crypto service

### The code

```csharp
using System;
using System.Threading.Tasks;
using pvWay.Crypto.Fw;

namespace CryptoFwConsole
{
    internal static class Program
    {
        private const string InitializationVectorString = "0123456789ABCDEF";
        private const string KeyString = "123456789 123456789 123456789 12";

        private static async Task Main(/*string[] args*/)
        {
            var crypto = new Crypto(
                KeyString,
                InitializationVectorString,
                TimeSpan.FromSeconds(10));

            var b64 = await crypto.EncryptAsync("test");
            Console.WriteLine(b64);
            var text = await crypto.DecryptAsync(b64);
            Console.WriteLine(text);

            var mc = new MyClass
            {
                TheHeader = "header",
                TheBody = "Body",
                TheFooter = "Footer"
            };

            b64 = await crypto.EncryptAsync(mc);
            Console.WriteLine(b64);
            var mcBack = await crypto.DecryptAsync<MyClass>(b64);
            Console.WriteLine(mcBack.TheHeader);
            Console.WriteLine(mcBack.TheBody);
            Console.WriteLine(mcBack.TheFooter);

            b64 = await crypto.EncryptEphemeralAsync(
                mc, TimeSpan.FromSeconds(15));
            mcBack = await crypto.DecryptEphemeralAsync<MyClass>(b64);
            Console.WriteLine(mcBack.TheBody + "is still valid");

            Console.ReadLine();
        }

        private class MyClass
        {
            public string TheHeader { get; set; }
            public string TheBody { get; set; }
            public string TheFooter { get; set; }
        }

    }
}

```

Happy coding
