using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class Currency : ICurrency
{
    public string? Code { get; }
    public string? Name { get; }
    public string? Plural { get; }
    public string? Symbol { get; }
    public string? SymbolNative { get; }

    public Currency(dynamic rd)
    {
        Code = rd.code;
        Name = rd.name;
        Plural = rd.plural;
        Symbol = rd.symbol;
        SymbolNative = rd.symbol_native;
    }
}