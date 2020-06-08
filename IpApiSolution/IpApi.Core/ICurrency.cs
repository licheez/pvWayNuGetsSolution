namespace pvWay.IpApi.Core
{
    public interface ICurrency
    {
        string Code { get; }
        string Name { get; }
        string Plural { get; }
        string Symbol { get; }
        string SymbolNative { get; }
    }
}