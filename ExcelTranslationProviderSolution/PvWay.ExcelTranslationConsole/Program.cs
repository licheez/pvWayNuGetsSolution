// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text.RegularExpressions;
using PvWay.ExcelTranslationProvider.nc8.Di;

Console.WriteLine("Hello, World!");

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