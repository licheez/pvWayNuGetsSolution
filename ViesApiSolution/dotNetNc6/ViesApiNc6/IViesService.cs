namespace pvWay.ViesApi.nc6;

public interface IViesService
{
    Task<IViesResult> CheckVatAsync(string countryCode, string vatNumber);
}