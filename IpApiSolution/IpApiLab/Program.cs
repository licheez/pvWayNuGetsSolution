using pvWay.IpApi.Core;
using System;

namespace IpApiLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var localizer = new Localizer("*****");
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


        //private static void TestIpApi()
        //{
        //    using var httpClient = new HttpClient();

        //    const string url = "https://ipapi.co/208.67.222.222/json/";
        //    var response = httpClient.GetAsync(url).Result;
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine("error");
        //    }

        //    var body = response.Content.ReadAsStringAsync().Result;
        //    dynamic rd = JsonConvert.DeserializeObject(body);
        //    if (rd == null) throw new Exception("rd should not be null");
        //    if (rd.error == true)
        //    {
        //        Console.WriteLine("Error");
        //        Console.WriteLine(rd.reason);
        //        Console.WriteLine(rd.message);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Ok");
        //        Console.WriteLine(rd.ip);
        //    }
        //}
    }
}
