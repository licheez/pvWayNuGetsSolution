# Excel Translations Provider for .Net Framework by pvWay

## Summary

The package contains two assets.

* A service that browses a given directory gathering excel files for building a translations dictionary.
* A cache singleton that refreshes when one Excel translation file in updated.

## Excel Translation file structure
The Excel Translation file should contain one WorkSheet with the following rows
* Row 1 (header row) :  KeyPart1 | KeyPart2 | KeyPart3 | KeyPart4 | IsoLanguageCode1 | IsoLanguageCode2...
* example
  * K1 | K2 |	K3 | K4 | en | fr |	nl
* Rows 2 to N : the keys and the corresponding translations
* example
  * Buttons | Add | . | . | Add | Ajouter | Toevoeggen

Based on the data here above the Translation Service will construct the following dictionary

{ 'Alpha.Buttons.Add', {'en':'Add',  'fr':'Ajouter', 'nl':'Toevoegen' }

Where
* 'Alpha' is the name of the Worksheet tab
* 'Button' is taken from row 2 (first data row), col 1
* Add is taken from row 2 (first data row), col 2
* row 2, col2 and col3 are empty and thereof not participating to the construction of the key

So the **key** is 'Alpha.Buttons.Add' and is case sensitive

For the values
* 'en' is taken from row1 (header row) col 5
* 'Add' is taken from row2 (data row) col 5
* 'fr' is taken from row1 (header row) col 6
* 'Ajouter' is taken from row2 (data row) col 6
* 'nl' is taken from row1 (header row) col 7
* 'Toevoegen' is taken from row2 (data row) col 7

The result is a IDicionary &lt;string, IDictionary &lt;string, string&gt;&gt;

## Usage

``` csharp
	
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
                (Exception e) => Console.WriteLine(e), // for logging any issue
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

```


### Interfaces

``` csharp
	
    public interface IExcelTranslationCache
    {
        /// <summary>
        /// key string should contain the keys separated by dots. example : 'enum.size'
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="keysString"></param>
        /// <returns></returns>
        string GetTranslation(string languageCode, string keysString);
    }
    
    /// <summary>
    /// This service provides the mechanism for managing a cached
    /// translation dictionary at data consumer side
    /// </summary>
    public interface IExcelTranslationService : IDisposable
    {
        /// <summary>
        /// With this date any data consumer can verify
        /// whether or not its cached copy of the dictionary
        /// is still up to date and if needed refresh the
        /// cache by getting the Translation property.
        /// </summary>
        /// <returns></returns>
        DateTime LastUpdateDateUtc { get; }

        /// <summary>
        /// The key is build from the concatenation of
        /// up to 4 key parts separated by dots. Example: 'components.buttons.save').
        /// The associated value is dictionary languageCode:string
        /// </summary>
        IDictionary<string, IDictionary<string, string>> Translations { get; }
    }    

```

Happy coding :-)
