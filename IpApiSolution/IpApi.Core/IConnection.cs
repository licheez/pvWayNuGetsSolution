namespace pvWay.IpApi.Core
{
    public interface IConnection
    {
        string Asn { get; }
        string Isp { get; }
    }
}