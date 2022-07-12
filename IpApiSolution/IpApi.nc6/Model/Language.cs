using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class Language : ILanguage
{
    public string? Code { get; }
    public string? Name { get; }
    public string? Native { get; }

    public Language(dynamic rd)
    {
        Code = rd.code;
        Name = rd.name;
        Native = rd.native;
    }
}