namespace pvWay.IpApi.Core
{
    public interface ILanguage
    {
        string Code { get; }
        string Name { get; }
        string Native { get; }
    }
}