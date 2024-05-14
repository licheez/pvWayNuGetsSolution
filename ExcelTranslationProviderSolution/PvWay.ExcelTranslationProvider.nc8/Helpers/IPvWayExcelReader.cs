namespace PvWay.ExcelTranslationProvider.nc8.Helpers;

public interface IPvWayExcelReader
{
    string SheetName { get; }
    string GetCellText(int rowIndex, int colIndex);
    int NbHeaderCols { get; }
    int RowCount { get; }
}