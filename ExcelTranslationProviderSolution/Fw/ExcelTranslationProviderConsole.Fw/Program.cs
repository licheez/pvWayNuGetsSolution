using System;
using pvWay.ExcelTranslationProvider.Fw;

namespace ExcelTranslationProviderConsole.Fw
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var excelTranslationService = new ExcelTranslationService(
                Console.WriteLine,
                "..\\..\\MyExcelTranslationsFolder",
                "trans_*.xlsx");
            var lastUpdateDate = excelTranslationService.LastUpdateDateUtc;
            Console.WriteLine($"last update date in folder = {lastUpdateDate :dd MMM yyyy HH:mm:ss}" );

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
