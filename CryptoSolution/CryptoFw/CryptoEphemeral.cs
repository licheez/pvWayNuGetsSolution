using System;

// ReSharper disable MemberCanBePrivate.Global
namespace pvWay.Crypto.Fw
{
    internal class CryptoEphemeral<T> : ICryptoEphemeral<T>
    {
        public DateTime ValidUntil { get; set; }
        public T Data { get; set; }

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
}