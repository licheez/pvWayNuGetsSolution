using System;
using pvWay.ExcelTranslationProvider.Fw;

namespace ExcelTranslationProviderConsole.Fw
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var excelTranslationService = new ExcelTranslationService(
                Console.WriteLine, // for logging any issue
                "..\\..\\MyExcelTranslationsFolder", // the path to the directory
                "trans_*.xlsx" // the filename skeleton
            );

            // we can get the file modification date of the most recent Excel
            var lastUpdateDate = excelTranslationService.LastUpdateDateUtc;
            Console.WriteLine($"last update date in folder = {lastUpdateDate:dd MMM yyyy HH:mm:ss}");

            // we can retrieve a IDictionary<string, IDictionary<string, string> 
            // containing all the translations
            var translations = excelTranslationService.ReadTranslations();
            foreach (var kvp in translations)
            {
                Console.Write($"{kvp.Key} =>");
                foreach (var innerKvp in kvp.Value)
                {
                    Console.Write($" {innerKvp.Key} - {innerKvp.Value} ");
                }
                Console.Write("\n");
            }

            // There is also a Cache Singleton that you can instantiate any where
            // When in place you get any translation for a given key
            var tc = ExcelTranslationCacheSingleton
                .GetInstance(excelTranslationService, TimeSpan.FromMinutes(5));

            var addButtonLiteral = tc.GetTranslation("en", "Alpha.buttons.add");

            Console.WriteLine(addButtonLiteral);

            Console.WriteLine("hit enter to quit");
            Console.ReadLine();
        }
    }
}
