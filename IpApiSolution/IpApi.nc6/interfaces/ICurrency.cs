namespace pvWay.IpApi.nc6.interfaces;

public interface ICurrency
{
    string? Code { get; }
    string? Name { get; }
    string? Plural { get; }
    string? Symbol { get; }
    string? SymbolNative { get; }
}