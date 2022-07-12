namespace pvWay.IpApi.nc6.interfaces;

public interface ILocalizer
{
    Task<ILocalizerResult> LocalizeAsync(string ip);
}