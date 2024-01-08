namespace PvWay.Crypto.nc8;

public interface ICryptoEphemeral<out T>
{
    DateTime ValidUntil { get; }
    T Data { get; }
}