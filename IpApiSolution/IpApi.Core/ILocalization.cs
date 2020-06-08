namespace pvWay.IpApi.Core
{
    public interface ILocalization
    {
        string Ip { get; }
        string HostName { get; }
        string Type { get; }
        string ContinentCode { get; }
        string ContinentName { get; }
        string CountryCode { get; }
        string CountryName { get; }
        string RegionCode { get; }
        string RegionName { get; }
        string City { get; }
        string Zip { get; }
        double Latitude { get; }
        double Longitude { get; }
        ILocation Location { get; }
        ITimeZone TimeZone { get; }
        ICurrency Currency { get; }
        IConnection Connection { get; }
        ISecurity Security { get; }
    }
}