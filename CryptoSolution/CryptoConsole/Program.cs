using System;
using pvWay.Crypto;

namespace CryptoConsole
{
    internal static class Program
    {
        private const string InitializationVectorString = "0123456789ABCDEF";
        private const string KeyString = "123456789 123456789 123456789 12";


        private static void Main(/*string[] args*/)
        {
            var crypto = new Crypto(KeyString, InitializationVectorString);

            var b64 = crypto.EncryptAsync("test").Result;
            Console.WriteLine(b64);
            var text = crypto.DecryptAsync(b64).Result;
            Console.WriteLine(text);

            var mc = new MyClass
            {
                TheHeader = "header",
                TheBody = "Body",
                TheFooter = "Footer"
            };

            b64 = crypto.EncryptAsync(mc).Result;
            Console.WriteLine(b64);
            var mcBack = crypto.DecryptAsync<MyClass>(b64).Result;
            Console.WriteLine(mcBack.TheHeader);
            Console.WriteLine(mcBack.TheBody);
            Console.WriteLine(mcBack.TheFooter);

        }

        private class MyClass
        {
            public string TheHeader { get; set; }
            public string TheBody { get; set; }
            public string TheFooter { get; set; }
        }

    }
}
