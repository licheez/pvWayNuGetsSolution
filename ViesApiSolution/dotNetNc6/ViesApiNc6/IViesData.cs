namespace pvWay.ViesApi.nc6;

public interface IViesData
{
    bool Valid { get; }
    string CountryCode { get; }
    string VatNumber { get; }
    string? Name { get; }
    string? Address { get; }
}