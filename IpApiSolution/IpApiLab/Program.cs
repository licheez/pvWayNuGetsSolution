using System;
using pvWay.IpApi.Core;

namespace IpApiLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var localizer = new Localizer("***************************");
            var localize = localizer.LocalizeAsync("****************").Result;
            if (localize.Failure)
            {
                Console.WriteLine(localize.Exception);
            }
            else
            {
                var loc = localize.Data;
                Console.WriteLine(loc.City);
            }
        }
    }
}
