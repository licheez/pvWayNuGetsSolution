namespace pvWay.IpApi.nc6.interfaces;

public interface ILocation
{
    int? GeoNameId { get; }
    string? Capital { get; }
    IEnumerable<ILanguage> Languages { get; }
    string? CountryFlagUrl { get; }
    string? CountryFlagEmoji { get; }
    string? CountryFlagEmojiUnicode { get; }
    string? CallingCode { get; }
    bool? EuroMember { get; }
}