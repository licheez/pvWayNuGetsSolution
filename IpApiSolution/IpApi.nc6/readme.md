# IpApi nuGet by pvWay

Provides geolocalization info for a given ip. This service is based on **IpStack** API.

You'll need a valid IpStack API key to use this nuGet service. 

Ip Stack offers a free API key that enables up to 10.000 requests per month.

Susbscribe here https://ipstack.com/product for getting your free API key

### Usage

#### Constructor

``` csharp
	
    // replace '****...' with your own key
    var localizer = new Localizer("*********************************");

```

### Localize

``` csharp
	
    var localize = localizer.LocalizeAsync("109.88.95.155").Result;
    if (localize.Failure)
    {
    	Console.WriteLine(localize.Exception);
    }
    else
    {
        var loc = localize.Data;
        Console.WriteLine(loc.City);
    }

```

#### Interfaces

``` csharp
	
    public interface ILocalizer
    {
        Task<<ILocalizerResult>> LocalizeAsync(string ip);
    }
    
    public interface ILocalizerResult
    {
        bool Success { get; }
        bool Failure { get; }
        Exception Exception { get; }
        ILocalization Data { get; }
    }

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
        double? Latitude { get; }
        double? Longitude { get; }
        ILocation Location { get; } // free plan
        ITimeZone TimeZone { get; } // as from basic plan
        ICurrency Currency { get; } // as from basic plan
        IConnection Connection { get; } // as from basic plan
        ISecurity Security { get; } // as from basic plan
    }
 
    public interface ILocation
    {
        int? GeoNameId { get; }
        string Capital { get; }
        IEnumerable<ILanguage> Languages { get; }
        string CountryFlagUrl { get; }
        string CountryFlagEmoji { get; }
        string CountryFlagEmojiUnicode { get; }
        string CallingCode { get; }
        bool? EuroMember { get; }
    }
 
    public interface ILanguage
    {
        string Code { get; }
        string Name { get; }
        string Native { get; }
    }
    
    public interface ITimeZone
    {
        string Id { get; }
        DateTime? CurrentTime { get; }
        int? GmtOffset { get; }
        string Code { get; }
        bool? IsDayLightSaving { get; }
    }

	public interface ICurrency
    {
        string Code { get; }
        string Name { get; }
        string Plural { get; }
        string Symbol { get; }
        string SymbolNative { get; }
    }
    
    public interface IConnection
    {
        string Asn { get; }
        string Isp { get; }
    }

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

```

Happy coding !

