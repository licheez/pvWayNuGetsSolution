namespace pvWay.ViesApi.Core
{
    public interface IViesData
    {
        bool Valid { get; }
        string CountryCode { get; }
        string VatNumber { get; }
        string Name { get; }
        string Address { get; }
    }
}