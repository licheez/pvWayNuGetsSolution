# Vies API for dotNet Core 6 by pvWay

Tiny async service that checks a VAT number against the European Vat Number database and returns whether or not the number is valid and - when available - the name and the address of the registered company

## Interfaces

### IViesResult

```csharp

    public interface IViesResult
    {
        bool Success { get; }
        bool Failure { get; }

        /// <summary>
        /// Exception is only set on Failure
        /// </summary>
        Exception? Exception { get; }

        /// <summary>
        /// Data is only set on Success
        /// </summary>
        IViesData? Data { get; }
    }
```

### IViesData

```csharp

    public interface IViesData
    {
        bool Valid { get; }
        string CountryCode { get; }
        string VatNumber { get; }
        string? Name { get; }
        string? Address { get; }
    }


```

### IViesService

```csharp
    public interface IViesService
    {
        Task<IViesResult> CheckVatAsync(
            string countryCode, string vatNumber);
    }
```

## Usage
```csharp
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
```
Happy Coding