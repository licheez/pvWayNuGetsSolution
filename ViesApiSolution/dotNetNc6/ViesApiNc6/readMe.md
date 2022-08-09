# Vies API for dotNet Core by pvWay

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
using System;
using pvWay.MethodResultWrapper.Core;
using pvWay.ViesApi.Core;

namespace ViesApiConsumer.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var viesService = new ViesService();
            var checkVat = viesService.CheckVatAsync("BE", "0459415853").Result;
            if (checkVat.Failure)
            {
                Console.WriteLine(checkVat.Exception);
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
Happy Coding