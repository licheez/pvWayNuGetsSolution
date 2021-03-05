namespace pvWay.IpApi.Core
{
    public interface ISecurity
    {
        bool? IsProxy { get; }
        string ProxyType { get; }
        bool? IsCrawler { get; }
        string CrawlerName { get; }
        string CrawlerType { get; }
        bool? IsTor { get; }
        string ThreatLevel { get; }
        string ThreatTypes { get; }
    }
}