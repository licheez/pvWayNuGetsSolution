using System;
using pvWay.MsSqlMultiPartVarChar.Core;

namespace MsSqlMultiPartVarCharLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string mpString =
                "en::english text::fr::texte en français avec le caractère \\: au milieu::nl::nederlandse tekst::";

            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            if (ok)
            {
                Console.WriteLine(res);
                Console.WriteLine(mpVarChar);
            }
            Console.ReadKey();

            var createScript = MpVarChar.CreateFunctionScript;
            Console.WriteLine(createScript);
            Console.ReadKey();
        }
    }
}
