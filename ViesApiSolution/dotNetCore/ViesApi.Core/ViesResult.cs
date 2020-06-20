using ViesWebService;

namespace pvWay.ViesApi.Core
{
    internal class ViesResult : IViesResult
    {
        public bool Valid { get; }
        public string CountryCode { get; }
        public string VatNumber { get; }
        public string Name { get; }
        public string Address { get; }

        public ViesResult(checkVatResponse r)
        {
            Valid = r.valid;
            CountryCode = r.countryCode;
            VatNumber = r.vatNumber;
            Name = r.name;
            Address = r.address;
        }
    }
}