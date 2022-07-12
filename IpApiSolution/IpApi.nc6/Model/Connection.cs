using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class Connection : IConnection
{
    public string? Asn { get; }
    public string? Isp { get; }

    public Connection(dynamic rd)
    {
        Asn = rd.asn;
        Isp = rd.isp;
    }
}