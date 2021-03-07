using System;
using pvWay.ExcelTranslationProvider.Fw;

namespace ExcelTranslationProviderConsole.Fw
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            // instantiate the Translation Service with a folder
            var excelTranslationService = new ExcelTranslationService(
                Console.WriteLine, // for logging any issue
                "..\\..\\MyExcelTranslationsFolder", // the path to the directory
                "trans_*.xlsx" // the filename skeleton
            );
            
            // we can get the file modification date of the most recent Excel
            var lastUpdateDate = excelTranslationService.LastUpdateDateUtc;
            Console.WriteLine($"last update date in folder = {lastUpdateDate :dd MMM yyyy HH:mm:ss}" );

            // we can retrieve a IDictionary<string, IDictionary<string, string> 
            // containing all the translations
            var translations = excelTranslationService.Translations;
            foreach (var kvp in translations)
            {
                Console.Write($"{kvp.Key} =>");
                foreach (var translation in kvp.Value)
                {
                    Console.Write($" {translation.Key} - {translation.Value} ");
                }
                Console.Write("\n");
            }

            // There is also a Cache Singleton that you can instantiate any where
            // When in place you get any translation for a given key
            ExcelTranslationCacheSingleton
                .GetInstance(
                    excelTranslationService, 
                    TimeSpan.FromMinutes(5),
                    translationCache =>
                    {
                        Console.WriteLine("translation cache is loaded");
                        var addButtonLiteral = translationCache
                            .GetTranslation("en", "Alpha.buttons.add");
                        Console.WriteLine(addButtonLiteral);
                        Console.WriteLine("hit any key to quit");
                    });
            
            Console.ReadKey();
        }
    }
}
