# Excel Translations Provider for .Net Core 8 by pvWay

## Summary

The package contains two assets.

* A service that browses a given directory or an assembly gathering excel files for building a translations dictionary.
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
* row 2, col2 and col3 are empty and thereof not participating in the construction of the key

So the **key** is 'Alpha.Buttons.Add' and is case-sensitive

For the values
* 'en' is taken from row1 (header row) col 5
* 'Add' is taken from row2 (data row) col 5
* 'fr' is taken from row1 (header row) col 6
* 'Ajouter' is taken from row2 (data row) col 6
* 'nl' is taken from row1 (header row) col 7
* 'Toevoegen' is taken from row2 (data row) col 7

The result is a IDictionary &lt;string, IDictionary &lt;string, string&gt;&gt;

## Injection and factories

The service architecture contains two layer
* The TranslationService that loads the ExcelSheets
* The TranslationCache that 
  * consumes the translation service
  * provides the translations 

### TranslationService implementations
#### Assembly Based
The Excel files are set as EmbeddedResources (i.e. will be stored inside the DLL)

<details>
<summary>See here the Injector for this implementation</summary>

```csharp
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayAssemblyBasedExcelTranslationDi
{
/// <summary>
/// Creates an instance of PvWayAssemblyExcelTranslationService for managing a cached translation dictionary at the data consumer side, based on an assembly and a regular expression.
/// </summary>
/// <param name="assembly">The assembly from which to retrieve the translation resources.</param>
/// <param name="resourceNameMatcher">A regular expression used to match the names of the translation resources.</param>
/// <param name="logException">An action to handle any exceptions that occur during the translation process.</param>
/// <returns>An instance of PvWayAssemblyExcelTranslationService.</returns>
public static IPvWayExcelTranslationService CreateAssemblyBasedExcelTranslationService(
Assembly assembly,
Regex resourceNameMatcher,
Action<Exception> logException)
{
var config = new PvWayAssemblyBasedExcelTranslationServiceConfig(
assembly, resourceNameMatcher, logException);
return new PvWayAssemblyExcelTranslationService(config);
}

    /// <summary>
    /// Adds an assembly based excel translation service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the translation service to.</param>
    /// <param name="assembly">The assembly from which to retrieve the translation resources.</param>
    /// <param name="resourceNameMatcher">A regular expression used to match the names of the translation resources.</param>
    /// <param name="logException">An action to handle any exceptions that occur during the translation process.</param>
    /// <param name="lifetime">The lifetime of the translation service.</param>
    public static void AddAssemblyBasedExcelTranslation(
        this IServiceCollection services,
        Assembly assembly,
        Regex resourceNameMatcher,
        Action<Exception> logException,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayAssemblyBasedExcelTranslationServiceConfig>(sp =>
            new PvWayAssemblyBasedExcelTranslationServiceConfig(
                assembly, resourceNameMatcher, logException));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationService),
            typeof(PvWayAssemblyExcelTranslationService), lifetime);
        services.Add(sd);
    }
}
```

</details>

#### External Folder Based
The Excel files are copied on the server

<details>
<summary>See here the injector for this implementation</summary>

```csharp
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayFolderBasedExcelTranslationDi
{
    /// <summary>
    /// Creates an instance of the PvWayFolderBasedExcelTranslationService class,
    /// which implements the IPvWayExcelTranslationService interface.
    /// </summary>
    /// <param name="excelFolderPath">The path to the folder that contains the Excel translation files.</param>
    /// <param name="fileWildCard">The wild card for the Excel translation file names.</param>
    /// <param name="logException">The action to be executed when an exception occurs.</param>
    /// <returns>An instance of the PvWayFolderBasedExcelTranslationService class.</returns>
    public static IPvWayExcelTranslationService CreateFolderBasedExcelTranslation(
            string excelFolderPath, 
            string fileWildCard, 
            Action<Exception> logException)
    {
        var config = new PvWayFolderBasedExcelTranslationServiceConfig(
            excelFolderPath, fileWildCard, logException);
        return new PvWayFolderBasedExcelTranslationService(config);
    }

    /// <summary>
    /// Adds the PvWayFolderBasedExcelTranslationService to the IServiceCollection
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to</param>
    /// <param name="excelFolderPath">The path to the folder that contains the Excel translation files</param>
    /// <param name="fileWildCard">The wild card for the Excel translation file names</param>
    /// <param name="logException">The action to be executed when an exception occurs</param>
    /// <param name="lifetime">The lifetime of the service (default is Singleton)</param>
    public static void AddPvWayFolderBasedExcelTranslation(
        this IServiceCollection services,
        string excelFolderPath, 
        string fileWildCard, 
        Action<Exception> logException, 
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayFolderBasedExcelTranslationServiceConfig>(_ =>
            new PvWayFolderBasedExcelTranslationServiceConfig(excelFolderPath, fileWildCard, logException));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationService),
            typeof(PvWayFolderBasedExcelTranslationService), lifetime);
        services.Add(sd);
    }
}
```

</details>

### Translation Cache

<details>
<summary>
See here after the injection for the Translation Cache
</summary>

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Services;

namespace PvWay.ExcelTranslationProvider.nc8.Di;

public static class PvWayTranslationCacheDi
{
    /// <summary>
    /// Creates an instance of IPvWayExcelTranslationCache.
    /// </summary>
    /// <param name="ts">The implementation of IPvWayExcelTranslationService.</param>
    /// <param name="rescanInterval">
    /// The time interval for re-scanning the folder for updated Excel files.
    /// set it to TimeSpan.Zero for disabling the rescan
    /// </param>
    /// <returns>An instance of IPvWayExcelTranslationCache.</returns>
    public static IPvWayExcelTranslationCache CreateTranslationCache(
            IPvWayExcelTranslationService ts,
            TimeSpan rescanInterval)
    {
        var config = new PvWayExcelTranslationCacheConfig(rescanInterval);
        return new PvWayExcelTranslationCache(ts, config);
    }

    /// <summary>
    /// Adds the PvWayTranslationCache to the ServiceCollection with the specified rescan interval and service lifetime.
    /// </summary>
    /// <param name="services">The ServiceCollection to add the PvWayTranslationCache to.</param>
    /// <param name="rescanInterval">
    /// The time interval for re-scanning the folder for updated Excel files.
    /// set it to TimeSpan.Zero for disabling the rescan
    /// </param>
    /// <param name="lifetime">The service lifetime (default: Singleton).</param>
    public static void AddPvWayTranslationCache(
        this ServiceCollection services,
        TimeSpan rescanInterval,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IPvWayExcelTranslationCacheConfig>(_ =>
            new PvWayExcelTranslationCacheConfig(rescanInterval));
        var sd = new ServiceDescriptor(
            typeof(IPvWayExcelTranslationCache),
            typeof(PvWayExcelTranslationCache),
            lifetime);
        services.Add(sd);
    } 
}
```

</details>

## Usage

See here after a small demo console that uses the service

```csharp
// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text.RegularExpressions;
using PvWay.ExcelTranslationProvider.nc8.Di;

Console.WriteLine("Hello, pvWay Excel Translation Cache");

var assembly = Assembly.GetExecutingAssembly();

Console.WriteLine("Assembly Based Translation Cache");
Console.WriteLine("================================");
var rx = new Regex("trans_.+\\.xlsx$");
var assemblyTs = PvWayAssemblyBasedExcelTranslationDi
    .CreateAssemblyBasedExcelTranslationService(assembly, rx, Console.WriteLine);
var assemblyTc = PvWayTranslationCacheDi.CreateTranslationCache(
    assemblyTs, TimeSpan.Zero);
var dt = assemblyTc.LastUpdateDateUtc;
Console.WriteLine($"{dt:s}");
var translations = assemblyTc.Translations;
foreach (var translation in translations)
{
    Console.Write(translation.Key + ":");
    foreach (var (key, value) in translation.Value)
    {
        Console.Write($"{key}: '{value}', ");
    }
    Console.WriteLine();
}
var cancelFr = assemblyTc.GetTranslation(
    "fr", "Alpha.buttons.cancel");
Console.WriteLine($"cancel in French: '{cancelFr}'");

Console.WriteLine();
Console.WriteLine("Folder Based Translation Cache");
Console.WriteLine("===============================");

var folderTs = PvWayFolderBasedExcelTranslationDi
    .CreateFolderBasedExcelTranslation(
        "../../../ExternalTranslationsFolder",
        "trans_*.xlsx", Console.WriteLine);
var folderTc = PvWayTranslationCacheDi.CreateTranslationCache(folderTs, TimeSpan.Zero);
dt = folderTc.LastUpdateDateUtc;
Console.WriteLine($"{dt:s}");
translations = folderTc.Translations;
foreach (var translation in translations)
{
    Console.Write(translation.Key + ":");
    foreach (var (key, value) in translation.Value)
    {
        Console.Write($"{key}: '{value}', ");
    }

    Console.WriteLine();
}

var cancelNl = folderTc.GetTranslation(
    "nl", "Alpha.buttons.cancel");
Console.WriteLine($"cancel in Dutch: '{cancelNl}'");

Console.WriteLine();
Console.WriteLine("Done");
```

Producing the following output

```
Assembly Based Translation Cache
================================
2024-05-14T09:13:32
Alpha.buttons.add:en: 'Add', fr: 'Ajouter', nl: 'Toevoegen', 
Alpha.buttons.cancel:en: 'Cancel', fr: 'Annuler', nl: 'Annuleren', 
Alpha.buttons.delete:en: 'Delete', fr: 'Supprimer', nl: 'Deleten', 
Alpha.buttons.edit:en: 'Edit', fr: 'Éditer', nl: 'Bewerken', 
Alpha.buttons.ok:en: 'Ok', fr: 'Ok', nl: 'Ok', 
Alpha.buttons.save:en: 'Save', fr: 'Sauver', nl: 'Opslaan', 
Top.header.anonymous:en: 'anonymous', fr: 'anonyme', nl: 'anonyme', 
Top.header.login:en: 'Login', fr: 'Se connecter', nl: 'Aanmelden', 
Top.header.logout:en: 'Logout', fr: 'Se déconnecter', nl: 'Afmelden', 
Top.header.version:en: 'Version', fr: 'Version', nl: 'Versie', 
cancel in French: 'Annuler'

Folder Based Translation Cache
===============================
2023-12-09T09:06:17
Alpha.buttons.add:en: 'Add', fr: 'Ajouter', nl: 'Toevoegen',
Alpha.buttons.cancel:en: 'Cancel', fr: 'Annuler', nl: 'Annuleren',
Alpha.buttons.delete:en: 'Delete', fr: 'Supprimer', nl: 'Deleten',
Alpha.buttons.edit:en: 'Edit', fr: 'Éditer', nl: 'Bewerken',
Alpha.buttons.ok:en: 'Ok', fr: 'Ok', nl: 'Ok',
Alpha.buttons.save:en: 'Save', fr: 'Sauver', nl: 'Opslaan',
Top.header.anonymous:en: 'anonymous', fr: 'anonyme', nl: 'anonyme',
Top.header.login:en: 'Login', fr: 'Se connecter', nl: 'Aanmelden',
Top.header.logout:en: 'Logout', fr: 'Se déconnecter', nl: 'Afmelden',
Top.header.version:en: 'Version', fr: 'Version', nl: 'Versie',
cancel in Dutch: 'Annuleren'

Done
```