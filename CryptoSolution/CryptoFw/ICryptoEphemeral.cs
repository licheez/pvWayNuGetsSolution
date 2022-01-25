using System;

namespace pvWay.Crypto.Fw
{
    public interface ICryptoEphemeral<out T>
    {
        DateTime ValidUntil { get; }
        T Data { get; }
    }
}