using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using ExcelDataReader;
using PvWay.ExcelTranslationProvider.nc8.Exceptions;

namespace PvWay.ExcelTranslationProvider.nc8.Helpers;

internal sealed class AssemblyBasedExcelReader : IPvWayExcelReader, IDisposable
{
    private readonly Action<Exception> _logException;
    private readonly Assembly _assembly;
    private readonly ImmutableArray<ImmutableArray<string>> _cells;
    private readonly string _sheetName;
    public string SheetName => _sheetName;
    
    public AssemblyBasedExcelReader(
        Action<Exception> logException,
        Assembly assembly,
        string excelResourceName)
    {
        _logException = logException;
        _assembly = assembly;
        _cells = LoadSheet(excelResourceName, out _sheetName);
    }

    private ImmutableArray<ImmutableArray<string>> LoadSheet(
        string resourceName, out string sheetName)
    {
        sheetName = string.Empty;
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var stream = _assembly.GetManifestResourceStream(resourceName);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // read the first worksheet
            sheetName = reader.Name;
            var rows = new List<ImmutableArray<string>>();
            
            // read header row
            var hasHeaderRow = reader.Read();
            if (!hasHeaderRow)
            {
                throw new PvWayExcelTranslationProviderException(
                    $"Excel file '{resourceName}' is empty");
            }

            var headerCols = new List<string>();
            // get the number of header columns
            // this will determine the number of column of the cell array
            var nbHeaderCols = reader.FieldCount;
            for (var c = 0; c < nbHeaderCols; c++)
            {
                var colName = reader.GetString(c);
                headerCols.Add(colName);
            }
            
            // let's add the header row to the cells
            rows.Add([..headerCols]);
            
            // process data rows
            while (reader.Read())
            {
                var dataCols = new List<string>();
                // Let's get the number of column in the current row.
                // Even if we only take the header column in consideration
                var nbDataCols = reader.FieldCount;
                
                // capture each row cell
                for (var c = 0; c < nbHeaderCols; c++)
                {
                    var val = c < nbDataCols
                        ? reader.GetString(c)
                        : string.Empty;
                    if (c == 0 && string.IsNullOrEmpty(val)) break;
                    dataCols.Add(val);
                }

                rows.Add([..dataCols]);
            }
            
            // time to return the rows
            return [..rows];
        }
        catch (Exception e)
        {
            var ex = new PvWayExcelTranslationProviderException(
                $"{e} while reading excel file {sheetName}");
            _logException(ex);
            throw;
        }
    }

    public string GetCellText(int rowIndex, int colIndex)
    {
        try
        {
            return _cells[rowIndex][colIndex];
        }
        catch (Exception e)
        {
            var ex = new PvWayExcelTranslationProviderException(
                $"{e} while getting cell text at " +
                $"rowIndex {rowIndex} and colIndex {colIndex}");
            _logException(ex);
            throw;
        }
    }

    public int NbHeaderCols => _cells[0].Length;
    public int RowCount => _cells.Length;
    
    public void Dispose()
    {
        // no operation
    }
}