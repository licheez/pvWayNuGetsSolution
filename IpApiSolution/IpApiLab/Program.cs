using System;
using pvWay.IpApi.Core;

namespace IpApiLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var localizer = new Localizer("e1a1fe9f2ad2d4e96d287750683f5cc8");
            var localize = localizer.LocalizeAsync("109.88.95.155").Result;
            if (localize.Failure)
            {
                Console.WriteLine(localize.ErrorMessage);
            }
            else
            {
                var loc = localize.Data;
                Console.WriteLine(loc.City);
            }
        }
    }
}
