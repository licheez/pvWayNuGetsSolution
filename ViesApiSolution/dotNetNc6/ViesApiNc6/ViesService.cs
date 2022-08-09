using ViesWebService;

namespace pvWay.ViesApi.nc6;

public class ViesService : IViesService
{
    public async Task<IViesResult> CheckVatAsync(string countryCode, string vatNumber)
    {
        if (string.IsNullOrEmpty(countryCode))
        {
            var err = new Exception("country code should not be null or empty");
            return new ViesResult(err);
        }
        if (string.IsNullOrEmpty(vatNumber))
        {
            var err = new Exception("vat number should not be null or empty");
            return new ViesResult(err);
        }
        vatNumber = vatNumber
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("-", "");

        var request = new checkVatRequest(countryCode, vatNumber);
        try
        {
            var vs = new checkVatPortTypeClient();
            var vr = await vs.checkVatAsync(request);
            var vd = new ViesData(vr);
            return new ViesResult(vd);
        }
        catch (Exception e)
        {
            return new ViesResult(e);
        }
    }
}