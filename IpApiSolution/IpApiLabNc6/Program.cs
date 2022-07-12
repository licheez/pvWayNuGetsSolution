using pvWay.IpApi.nc6;

var localizer = new Localizer("***********************");
var localize = await localizer.LocalizeAsync("86.105.245.69");
if (localize.Failure)
{
    Console.WriteLine(localize.Exception);
}
else
{
    var loc = localize.Data!;
    Console.WriteLine(loc.CountryName);
}
