using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace PvWay.Crypto.nc8;

internal sealed class Crypto : ICrypto
{
    private readonly TimeSpan _defaultValidity;
    private readonly byte[] _iv;
    private readonly byte[] _key;
    private readonly Aes _aes;

    /// <summary>
    /// Initialize the Crypto class. Provide a 32 char key and a 16 char iv
    /// </summary>
    /// <param name="keyString">should be exactly 32 characters long</param>
    /// <param name="initializationVectorString">should be exactly 16 characters long</param>
    /// <param name="defaultValidity">default validity for ephemeral encryption</param>
    public Crypto(
        string keyString,
        string initializationVectorString,
        TimeSpan defaultValidity)
    {
        if (keyString.Length != 32)
            throw new PvWayCryptoException(
                "invalid key (should be 32  char long");
        if (initializationVectorString.Length != 16)
            throw new PvWayCryptoException(
                "invalid initialization vector (should be 16 char long");
        _defaultValidity = defaultValidity;
        _aes = Aes.Create();
        if (_aes == null)
            throw
                new PvWayCryptoException(
                    "aes should not be null");

        _key = Encoding.UTF8.GetBytes(keyString);
        _iv = Encoding.ASCII.GetBytes(initializationVectorString);
    }

    public async Task<string> EncryptStringAsync(string text)
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

    public async Task<string> EncryptObjectAsync<T>(T data) where T : class
    {
        var json = JsonConvert.SerializeObject(data);
        return await EncryptStringAsync(json);
    }


    public async Task<string> EncryptEphemeralStringAsync(
        string text, TimeSpan? validity = null)
    {
        var ce = new CryptoEphemeral<string>(
            text, validity ?? _defaultValidity);
        return await EncryptObjectAsync(ce);
    }

    public async Task<string> EncryptEphemeralObjectAsync<T>(
        T data, TimeSpan? validity = null)
        where T : class
    {
        var ce = new CryptoEphemeral<T>(
            data, validity ?? _defaultValidity);
        return await EncryptObjectAsync(ce);
    }


    public async Task<string> DecryptStringAsync(string base64Str)
    {
        var buffer = Convert.FromBase64String(base64Str);
        var dt = _aes.CreateDecryptor(_key, _iv);
        await using var ms = new MemoryStream(buffer);
        await using var cs = new CryptoStream(ms, dt, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        var text = await sr.ReadToEndAsync();
        return text;
    }

    public async Task<T> DecryptObjectAsync<T>(string base64Str)
        where T : class
    {
        var json = await DecryptStringAsync(base64Str);
        return JsonConvert.DeserializeObject<T>(json)!;
    }


    public async Task<string?> DecryptEphemeralStringAsync(string base64Str)
    {
        var ce = await DecryptObjectAsync<CryptoEphemeral<string>>(base64Str);
        return ce.ValidUntil > DateTime.UtcNow
            ? ce.Data
            : null;
    }

    public async Task<T?> DecryptEphemeralObjectAsync<T>(string base64Str) where T : class
    {
        var ce = await DecryptObjectAsync<CryptoEphemeral<T>>(base64Str);
        return ce.ValidUntil > DateTime.UtcNow ? ce.Data : null;
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