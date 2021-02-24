using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

//using OfficeOpenXml;

namespace pvWay.ExcelTranslationProvider.Fw
{
    internal class ExcelReader : IDisposable
    {
        private readonly Action<Exception> _log;
        private readonly XSSFWorkbook _wb;
        private readonly ISheet _ws;

        public int RowCount => _ws.LastRowNum;
        public string TabName => _ws.SheetName;

        public ExcelReader(
            Action<Exception> log,
            string excelFilePath)
        {
            _log = log;
            var fi = new FileInfo(excelFilePath);
            var sr = fi.OpenRead();
            _wb = new XSSFWorkbook(sr);
            _ws = _wb.GetSheetAt(0);
        }

        public string GetCellText(int rowNumber, int columnNumber)
        {
            try
            {
                var row = _ws.GetRow(rowNumber);
                var cell = row.GetCell(columnNumber);
                var text = cell?.StringCellValue;
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
            _wb?.Close();
        }
    }
}
