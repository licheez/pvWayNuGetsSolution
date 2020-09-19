using System;
using pvWay.ViesApi.Core;

namespace ViesApiLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var viesService = new ViesService();
            var checkVat = viesService.CheckVatAsync(
                "BE", "0459415853").Result;
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