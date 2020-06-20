# Vies API for dotNet Core by pvWay

Tiny async service that checks a VAT number against the European Vat Number database and returns whether or not the number is valid and - when available - the name and the address of the registered company

## Interfaces

### IViesResult

```csharp
public interface IViesResult
{
  bool Valid { get; }
  string CountryCode { get; }
  string VatNumber { get; }
  string Name { get; }
  string Address { get; }
}
```

### IViesService

```csharp
    public interface IViesService
    {
        Task<IMethodResult<IViesResult>> CheckVatAsync(
        string countryCode, string vatNumber);
    }
```

## Dependencies

This package is using the [MethodResult.Core nuGet package](https://www.nuget.org/packages/pvWay.MethodResultWrapper.Core/)


## Usage
```csharp
using System;
using pvWay.MethodResultWrapper.Core;
using pvWay.ViesApi.Core;

namespace ViesApiConsumer.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var ls = new ConsoleLogger();
            var viesService = new ViesService(ls);
            var checkVat = viesService.CheckVatAsync("BE", "0459415853").Result;
            if (checkVat.Failure)
            {
                Console.WriteLine(checkVat.ErrorMessage);
            }
            else
            {
                var viesRes = checkVat.Data;
                Console.WriteLine(viesRes.Valid);
                Console.WriteLine(viesRes.CountryCode);
                Console.WriteLine(viesRes.VatNumber);
                Console.WriteLine(viesRes.Name);
                Console.WriteLine(viesRes.Address);
            }
        }
    }
}
```
