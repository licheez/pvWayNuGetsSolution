using System.Text;
using PvWay.ExcelTranslationProvider.nc8.Helpers;

namespace PvWay.ExcelTranslationProvider.nc8.Model;

internal interface IExcelDataRow
{
    string Key { get; }
    IDictionary<string, string> Translations { get; }
}

internal sealed class ExcelDataRow: IExcelDataRow
{
    public string Key { get; }
    public IDictionary<string, string> Translations { get; }

    public ExcelDataRow(
        IPvWayExcelReader er,
        IDictionary<string, int> langMap,
        int rowIndex)
    {
        var kParts = new[]
        {
            er.SheetName,
            er.GetCellText(rowIndex, 0),
            er.GetCellText(rowIndex, 1),
            er.GetCellText(rowIndex, 2),
            er.GetCellText(rowIndex, 3)
        };
        var sb = new StringBuilder();
        foreach (var kPart in kParts)
        {
            if (string.IsNullOrEmpty(kPart)) break;
            if (sb.Length > 0) sb.Append('.');
            sb.Append(kPart);
        }
        Key = sb.ToString();
        Translations = new Dictionary<string, string>();
        foreach (var (langCode, langCol) in langMap)
        {
            var translation = er.GetCellText(rowIndex, langCol);
            Translations.Add(langCode, translation);
        }

    }

    public override string ToString()
    {
        var sb = new StringBuilder($"{Key}:");
        foreach (var (langCode, translation) in Translations)
        {
            sb.Append($"{langCode}-{translation}");
        }
        return sb.ToString();
    }

}