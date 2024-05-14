﻿using System.Collections.Immutable;
using System.Text;
using ExcelDataReader;
using PvWay.ExcelTranslationProvider.nc8.Exceptions;

namespace PvWay.ExcelTranslationProvider.nc8.Helpers
{
    internal sealed class FolderBasedExcelReader : IPvWayExcelReader, IDisposable
    {
        private readonly Action<Exception> _log;
        private readonly ImmutableArray<ImmutableArray<string>> _cells;

        private readonly string _sheetName;
        public string SheetName => _sheetName;

        public FolderBasedExcelReader(
            Action<Exception> log,
            string excelFilePath)
        {
            _log = log;
            _cells = LoadSheet(excelFilePath, out _sheetName);
        }

        private ImmutableArray<ImmutableArray<string>> LoadSheet(
            string excelFilePath,
            out string sheetName)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var stream = File.OpenRead(excelFilePath);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                // read first worksheet
                sheetName = reader.Name;
                var rows = new List<ImmutableArray<string>>();

                // read header row
                var hasHeaderRow = reader.Read();
                if (!hasHeaderRow)
                    throw new PvWayExcelTranslationProviderException(
                        "Excel file is empty");
                var headerCols = new List<string>();

                // get the number of header columns
                // this will determine the number of columns of the 
                // cell array
                var nbHeaderCols = reader.FieldCount;
                for (var c = 0; c < nbHeaderCols; c++)
                {
                    var val = reader.GetString(c);
                    headerCols.Add(val);
                }

                // let's add the header row the the cells
                rows.Add([..headerCols]);

                // process data rows
                while (reader.Read())
                {
                    var dataCols = new List<string>();
                    // let's get the number of column in the current row
                    // however the number of columns to browse
                    // is determined by the header row
                    var nbDataCols = reader.FieldCount;
                    
                    // iterate columns
                    for (var c = 0; c < nbHeaderCols; c++)
                    {
                        var val = c < nbDataCols
                            ? reader.GetString(c)
                            : string.Empty;
                        if (c == 0 && string.IsNullOrEmpty(val)) break;

                        dataCols.Add(val);
                    }
                    // add the data row to the cells
                    rows.Add([..dataCols]);
                }

                // return the rows 
                return [..rows];

            }
            catch (Exception e)
            {
                _log(e);
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
                _log(e);
                throw;
            }
        }

        public int NbHeaderCols => _cells[0].Length;

        public int RowCount => _cells.Length;


        public void Dispose()
        {
        }
    }
}
