using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace pvWay.ExcelTranslationProvider.Fw
{
    internal class ExcelReader : IDisposable
    {
        private readonly Action<Exception> _log;
        private readonly ExcelPackage _excelPackage;

        private readonly ExcelWorksheet _ws;

        public int RowCount => _ws.Dimension.Rows;
        public string TabName => _ws.Name;

        public ExcelReader(
            Action<Exception> log,
            string excelFilePath)
        {
            _log = log;
            var fi = new FileInfo(excelFilePath);
            _excelPackage = new ExcelPackage(fi);
            var wb = _excelPackage.Workbook;
            _ws = wb.Worksheets.First();
        }

        public string GetCellText(int rowNumber, int columnNumber)
        {
            try
            {
                var cell = _ws.Cells[rowNumber, columnNumber];
                var text = cell?.Text;
                return text;
            }
            catch (Exception e)
            {
                _log(e);
                throw;
            }
        }

        public void Dispose()
        {
            _excelPackage?.Dispose();
        }
    }
}
