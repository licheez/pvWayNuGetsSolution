namespace pvWay.IpApi.Core
{
    internal class Connection : IConnection
    {
        public string Asn { get; }
        public string Isp { get; }

        public Connection(dynamic rd)
        {
            Asn = rd.asn;
            Isp = rd.isp;
        }
    }
}