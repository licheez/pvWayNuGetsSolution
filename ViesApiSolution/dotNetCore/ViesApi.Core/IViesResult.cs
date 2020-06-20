namespace pvWay.ViesApi.Core
{
    public interface IViesResult
    {
        bool Valid { get; }
        string CountryCode { get; }
        string VatNumber { get; }
        string Name { get; }
        string Address { get; }
    }
}