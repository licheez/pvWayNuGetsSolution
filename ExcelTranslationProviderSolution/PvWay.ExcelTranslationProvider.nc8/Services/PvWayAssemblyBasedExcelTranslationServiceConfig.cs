using System.Reflection;
using System.Text.RegularExpressions;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;

namespace PvWay.ExcelTranslationProvider.nc8.Services;

internal class PvWayAssemblyBasedExcelTranslationServiceConfig(
    Assembly assembly,
    Regex resourceNameMatcher,
    Action<Exception> logException)
    : IPvWayAssemblyBasedExcelTranslationServiceConfig
{
    public Regex ResourceNameMatcher { get; } = resourceNameMatcher;
    public Assembly Assembly { get; } = assembly;
    public Action<Exception> LogException { get; } = logException;
}