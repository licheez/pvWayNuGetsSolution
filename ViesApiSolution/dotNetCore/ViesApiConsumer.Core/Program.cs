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
