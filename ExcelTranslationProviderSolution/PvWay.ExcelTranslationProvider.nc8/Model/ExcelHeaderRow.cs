using System.Collections;
using PvWay.ExcelTranslationProvider.nc8.Helpers;

namespace PvWay.ExcelTranslationProvider.nc8.Model;

internal interface IExcelHeaderRow
{
    IDictionary<string, int> LangMap { get; }
}

internal class ExcelHeaderRow: IExcelHeaderRow
{
    public IDictionary<string, int> LangMap { get; }

    public ExcelHeaderRow(IPvWayExcelReader xr)
    {
        LangMap = new Dictionary<string, int>();
        for (var col = 4; col < xr.NbHeaderCols; col++)
        {
            var langCode = xr.GetCellText(0, col);
            LangMap.Add(langCode, col);
        }
    }
}