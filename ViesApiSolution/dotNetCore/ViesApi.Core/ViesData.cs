using System;
using ViesWebService;

namespace pvWay.ViesApi.Core
{
    internal class ViesData : IViesData
    {
        public bool Valid { get; }
        public string CountryCode { get; }
        public string VatNumber { get; }
        public string Name { get; }
        public string Address { get; }

        public ViesData(checkVatResponse r)
        {
            Valid = r.valid;
            CountryCode = r.countryCode;
            VatNumber = r.vatNumber;
            Name = r.name;
            Address = r.address;
        }
    }

    internal class ViesResult : IViesResult
    {
        public bool Success { get; }
        public bool Failure => !Success;
        public Exception Exception { get; }
        public IViesData Data { get; }

        public ViesResult(IViesData data)
        {
            Success = true;
            Exception = null;
            Data = data;
        }

        public ViesResult(Exception e)
        {
            Success = false;
            Exception = e;
            Data = null;
        }
    }
}