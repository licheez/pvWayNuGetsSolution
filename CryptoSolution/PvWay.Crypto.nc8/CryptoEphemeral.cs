// ReSharper disable MemberCanBePrivate.Global
namespace PvWay.Crypto.nc8;

internal sealed class CryptoEphemeral<T> : ICryptoEphemeral<T>
{
    public DateTime ValidUntil { get; set; }
    public T Data { get; set; } = default!;

    // ReSharper disable once UnusedMember.Global
    public CryptoEphemeral()
    {
    }

    public CryptoEphemeral(T data, TimeSpan validity)
    {
        ValidUntil = DateTime.UtcNow + validity;
        Data = data;
    }
}