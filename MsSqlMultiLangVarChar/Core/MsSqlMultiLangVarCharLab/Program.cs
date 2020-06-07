using System;
using pvWay.MsSqlMultiPartVarChar.Core;

namespace MsSqlMultiPartVarCharLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string mpString = "<en>bear</en><fr>ours</fr>";

            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            if (ok)
            {
                Console.WriteLine(res);
                Console.WriteLine(mpVarChar);
            }
            Console.ReadKey();

            var createScript = MpVarChar.CreateFunctionScript("dbo", "FnGetMpPart");
            Console.WriteLine(createScript);
            Console.ReadKey();
        }
    }
}
