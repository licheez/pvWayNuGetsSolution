namespace pvWay.Crypto.nc6;

internal interface ICryptoEphemeral<out T>
{
    DateTime ValidUntil { get; }
    T Data { get; }
}