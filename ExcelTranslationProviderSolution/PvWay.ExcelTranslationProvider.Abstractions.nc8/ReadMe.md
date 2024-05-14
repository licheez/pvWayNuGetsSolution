# pvWay Excel Translations Provider for dotNet Core 8

 ## Description

This package provide the abstraction layer for the pvWayExcelTranslationProvider package.

## Content

### Cache
Usually implemented as a singleton it maintains the array of translations in cache.

```csharp
namespace PvWay.ExcelTranslationProvider.Abstractions.nc8
{
    public interface IPvWayExcelTranslationCache
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

        /// <summary>
        /// Stop re-scanning the folder
        /// </summary>
        void StopRescan();
        
        /// <summary>
        /// Re-scan the folder for updated Excel files
        /// </summary>
        void RefreshNow();

        /// <summary>
        /// key string should contain the keys separated by dots. example : 'enum.size'
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="keysString"></param>
        /// <returns></returns>
        string GetTranslation(string languageCode, string keysString);
    }

}```
### Cache loading

For loading the cache you need to inject an implementation of the IPvWayExcelTranslationService.

```csharp
namespace PvWay.ExcelTranslationProvider.Abstractions.nc8
{
    /// <summary>
    /// This service provides the mechanism for managing a cached
    /// translation dictionary at data consumer side
    /// </summary>
    public interface IPvWayExcelTranslationService : IDisposable
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
        IDictionary<string, IDictionary<string, string>> ReadTranslations();
    }
}
```

There are two flavour for this service: 
* Assembly based
* File system based

### ExcelTranslationService using Excel as embedded resources in a given assembly

```csharp
using System.Reflection;
using System.Text.RegularExpressions;

namespace PvWay.ExcelTranslationProvider.Abstractions.nc8;


public interface IPvWayAssemblyBasedExcelTranslationServiceConfig
{
    /// <summary>
    /// This will be used for selecting Excel resources from
    /// the collection of resource names
    /// Example: "tr.+\\.xlsx$" for getting all resources
    /// where the name starts with "tr." and ends with ".xlsx"
    /// </summary>
    Regex ResourceNameMatcher { get; }
    /// <summary>
    /// The assembly containing the Excel resources
    /// </summary>
    Assembly Assembly { get; }
    
    Action<Exception> LogException { get; } 
}
```
### ExcelTranslationService using Excel files on file system

```csharp
namespace PvWay.ExcelTranslationProvider.Abstractions.nc8;

public interface IPvWayFolderBasedExcelTranslationServiceConfig
{
    /// <summary>
    /// The external folder containing the translation Excel 
    /// </summary>
    string ExcelFolderPath { get; }
    /// <summary>
    /// A wildcard such as "tr_*.xls?"
    /// </summary>
    string FileWildCard { get; }
    /// <summary>
    /// A delegate for logging exceptions
    /// </summary>
    Action<Exception> LogException { get; }
}
```
