using Newtonsoft.Json;
using pvWay.MethodResultWrapper.Model;
using System;

namespace MethodResultWrapperConsumer
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var hRes = new DsoHttpResult<string>("hello");
            var jRes = JsonConvert.SerializeObject(hRes);
            var dRes = JsonConvert.DeserializeObject<DsoHttpResult<string>>(jRes);
            Console.WriteLine(dRes.Data);
            Console.ReadKey();
        }
    }
}
