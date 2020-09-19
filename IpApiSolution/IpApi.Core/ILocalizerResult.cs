using System;

namespace pvWay.IpApi.Core
{
    public interface ILocalizerResult
    {
        bool Success { get; }
        bool Failure { get; }
        Exception Exception { get; }
        ILocalization Data { get; }
    }
}