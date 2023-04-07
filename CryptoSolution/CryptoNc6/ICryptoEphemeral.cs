namespace pvWay.Crypto.nc6
{
    public interface ICryptoEphemeral<out T>
    {
        DateTime ValidUntil { get; }
        T Data { get; }
    }
}