using pvWay.ViesApi.nc6;

Console.WriteLine("ViesApiLab Nc6");
Console.WriteLine("--------------");

var viesService = new ViesService();
var checkVat = await viesService
    .CheckVatAsync("BE", "0459 415 853");
if (checkVat.Failure)
{
    Console.WriteLine(checkVat.Exception);
}
else
{
    var viesRes = checkVat.Data!;
    Console.WriteLine(viesRes.Valid);
    Console.WriteLine(viesRes.CountryCode);
    Console.WriteLine(viesRes.VatNumber);
    Console.WriteLine(viesRes.Name);
    Console.WriteLine(viesRes.Address);
}
