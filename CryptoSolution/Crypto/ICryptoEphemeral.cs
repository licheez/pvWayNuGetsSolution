using System;

namespace pvWay.Crypto.Core
{
    public interface ICryptoEphemeral<out T>
    {
        DateTime ValidUntil { get; }
        T Data { get; }
    }
}