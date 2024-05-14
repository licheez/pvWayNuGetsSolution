using System.Reflection;
using System.Text.RegularExpressions;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Helpers;
using PvWay.ExcelTranslationProvider.nc8.Model;

namespace PvWay.ExcelTranslationProvider.nc8.Services;

internal sealed class PvWayAssemblyExcelTranslationService(
    IPvWayAssemblyBasedExcelTranslationServiceConfig config)
    : IPvWayExcelTranslationService
{
    private readonly Action<Exception> _logException = config.LogException; 
    private readonly Assembly _assembly = config.Assembly;
    private readonly Regex _nameMatcher = config.ResourceNameMatcher;
    
    public void Dispose()
    {
        // no operation
    }

    public DateTime LastUpdateDateUtc => _assembly.GetBuildDateUtc();
    
    public IDictionary<string, IDictionary<string, string>> ReadTranslations()
    {
        try
        {
            var resourceNames = _assembly.GetManifestResourceNames()
                .ToList();
            var excelResourceNames = resourceNames
                .Where(x => _nameMatcher.IsMatch(x))
                .ToList();
            var rows = new List<IExcelDataRow>();
            foreach (var resourceName in excelResourceNames)
            {
                using var xReader = new AssemblyBasedExcelReader(
                    _logException, _assembly, resourceName);
                var nbRows = xReader.RowCount;
                var header = new ExcelHeaderRow(xReader);
                var langMap = header.LangMap;
                // start at row 1 to skip the header
                for (var r = 1; r < nbRows; r++)
                {
                    var row = new ExcelDataRow(xReader, langMap, r);
                    rows.Add(row);
                }
            }

            return rows.ToDictionary(
                k => k.Key, v => v.Translations);
        }
        catch (Exception e)
        {
            _logException(e);
            throw;
        }
    }
    
}