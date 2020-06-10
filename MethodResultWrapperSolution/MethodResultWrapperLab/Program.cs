using Newtonsoft.Json;
using pvWay.MethodResultWrapper.Model;
using System;

namespace MethodResultWrapperLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var hRes = new DsoHttpResult<string>("hello");
            var json = JsonConvert.SerializeObject(hRes);
            var dRes = JsonConvert.DeserializeObject<DsoHttpResult<string>>(json);
            Console.WriteLine(dRes.Data);
            Console.ReadKey();
        }
    }
}
