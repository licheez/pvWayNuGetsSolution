using System;

namespace pvWay.IpApi.Core
{
    internal class LocalizerResult : ILocalizerResult
    {
        public bool Success { get; }
        public bool Failure => !Success;
        public Exception Exception { get; }
        public ILocalization Data { get; }

        private LocalizerResult(Exception e)
        {
            Success = false;
            Exception = e;
            Data = null;
        }

        private LocalizerResult(ILocalization data)
        {
            Success = true;
            Exception = null;
            Data = data;
        }

        public static ILocalizerResult Failed(Exception e)
        {
            return new LocalizerResult(e);
        }

        public static ILocalizerResult Succeeded(ILocalization data)
        {
            return new LocalizerResult(data);
        }
    }
}
