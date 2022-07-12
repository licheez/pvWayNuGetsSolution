using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class Localization : ILocalization
{

    public string Ip { get; }
    public string HostName { get; }
    public string Type { get; }
    public string ContinentCode { get; }
    public string ContinentName { get; }
    public string CountryCode { get; }
    public string CountryName { get; }
    public string RegionCode { get; }
    public string RegionName { get; }
    public string City { get; }
    public string Zip { get; }
    public double? Latitude { get; }
    public double? Longitude { get; }
    public ILocation Location { get; }
    public ITimeZone TimeZone { get; }
    public ICurrency Currency { get; }
    public IConnection Connection { get; }
    public ISecurity Security { get; }

    public Localization(dynamic rd)
    {
        Ip = rd.ip;
        HostName = rd.hostname;
        Type = rd.type;
        ContinentCode = rd.continent_code;
        ContinentName = rd.continent_name;
        CountryCode = rd.country_code;
        CountryName = rd.country_name;
        RegionCode = rd.region_code;
        RegionName = rd.region_name;
        City = rd.city;
        Zip = rd.zip;
        Latitude = rd.latitude;
        Longitude = rd.longitude;
        Location = new Location(rd.location);
        if (rd.time_zone != null)
            TimeZone = new TimeZone(rd.time_zone);
        if (rd.currency != null)
            Currency = new Currency(rd.currency);
        if (rd.connection != null)
            Connection = new Connection(rd.connection);
        if (rd.security != null)
            Security = new Security(rd.security);
    }

}