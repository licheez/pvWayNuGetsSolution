using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class Security : ISecurity
{
    public bool? IsProxy { get; }
    public string? ProxyType { get; }
    public bool? IsCrawler { get; }
    public string? CrawlerName { get; }
    public string? CrawlerType { get; }
    public bool? IsTor { get; }
    public string? ThreatLevel { get; }
    public string? ThreatTypes { get; }

    public Security(dynamic rd)
    {
        IsProxy = rd.is_proxy;
        ProxyType = rd.proxy_type;
        IsCrawler = rd.is_crawler;
        CrawlerName = rd.crawler_name;
        CrawlerType = rd.crawler_type;
        IsTor = rd.is_tor;
        ThreatLevel = rd.threat_level;
        ThreatTypes = rd.threat_types;
    }
}