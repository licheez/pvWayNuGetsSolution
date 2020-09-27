using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pvWay.Crypto.Core
{
    public class Crypto: ICrypto
    {   
        readonly byte[] _iv;
        private readonly byte[] _key;
        private readonly Aes _aes;

        /// <summary>
        /// Initialize the Crypto class. Provide a 32 char key and a 16 char iv
        /// </summary>
        /// <param name="keyString">should be exactly 32 characters long</param>
        /// <param name="initializationVectorString">should be exactly 16 characters long</param>
        public Crypto(string keyString, string initializationVectorString)
        {
            if (keyString.Length != 32) 
                throw new Exception("invalid key (should be 32  char long");
            if (initializationVectorString.Length != 16) 
                throw new Exception("invalid initialization vector (should be 16 char long");
            _aes = Aes.Create();
            if (_aes == null) throw new Exception("aes should not be null");

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.ASCII.GetBytes(initializationVectorString);
        }


        public async Task<string> EncryptAsync(string text)
        {
            var ct = _aes.CreateEncryptor(_key, _iv);
            await using var ms = new MemoryStream();
            await using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            await using var sw = new StreamWriter(cs);
            await sw.WriteAsync(text);
            
            sw.Close();
            cs.Close();
            ms.Close();

            var buffer = ms.ToArray();
            var b64Str = Convert.ToBase64String(buffer);
            return b64Str;
        }

        public async Task<string> EncryptAsync<T>(T data) where T: class
        {
            var json = JsonConvert.SerializeObject(data);
            return await EncryptAsync(json);
        }


        public async Task<string> DecryptAsync(string b64Str)
        {
            var buffer = Convert.FromBase64String(b64Str);
            var dt = _aes.CreateDecryptor(_key, _iv);
            await using var ms = new MemoryStream(buffer);
            await using var cs = new CryptoStream(ms, dt, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            var text = await sr.ReadToEndAsync();
            return text;
        }

        public async Task<T> DecryptAsync<T>(string b64Str) where T:class
        {
            var json = await DecryptAsync(b64Str);
            return JsonConvert.DeserializeObject<T>(json);
        }


        public void Dispose()
        {
            _aes.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(Dispose);
        }
    }
}
