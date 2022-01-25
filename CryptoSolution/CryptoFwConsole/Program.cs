using System;
using System.Threading.Tasks;
using pvWay.Crypto.Fw;

namespace CryptoFwConsole
{
    internal static class Program
    {
        private const string InitializationVectorString = "0123456789ABCDEF";
        private const string KeyString = "123456789 123456789 123456789 12";

        private static async Task Main(/*string[] args*/)
        {
            var crypto = new Crypto(
                KeyString,
                InitializationVectorString,
                TimeSpan.FromSeconds(10));

            var b64 = await crypto.EncryptAsync("test");
            Console.WriteLine(b64);
            var text = await crypto.DecryptAsync(b64);
            Console.WriteLine(text);

            var mc = new MyClass
            {
                TheHeader = "header",
                TheBody = "Body",
                TheFooter = "Footer"
            };

            b64 = await crypto.EncryptAsync(mc);
            Console.WriteLine(b64);
            var mcBack = await crypto.DecryptAsync<MyClass>(b64);
            Console.WriteLine(mcBack.TheHeader);
            Console.WriteLine(mcBack.TheBody);
            Console.WriteLine(mcBack.TheFooter);

            b64 = await crypto.EncryptEphemeralAsync(
                mc, TimeSpan.FromSeconds(15));
            mcBack = await crypto.DecryptEphemeralAsync<MyClass>(b64);
            Console.WriteLine(mcBack.TheBody + "is still valid");

            Console.ReadLine();
        }

        private class MyClass
        {
            public string TheHeader { get; set; }
            public string TheBody { get; set; }
            public string TheFooter { get; set; }
        }

    }
}
