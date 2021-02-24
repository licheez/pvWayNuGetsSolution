using System;
using System.IO;
using OfficeOpenXml;

namespace pvWay.ExcelTranslationProvider.Fw
{
    internal class ExcelReader : IDisposable
    {
        private readonly Action<Exception> _log;
        private readonly ExcelPackage _excelPackage;
        private readonly FileInfo _fi;

        public ExcelReader(
            Action<Exception> log,
            string excelFilePath)
        {
            _log = log;
            _fi = new FileInfo(excelFilePath);
            _excelPackage = new ExcelPackage(_fi);
        }

        public string BaseFileName => Path.GetFileNameWithoutExtension(_fi.Name);

        public string GetCellText(int workSheetIndex,
            int rowNumber, int columnNumber)
        {
            try
            {
                var wb = _excelPackage.Workbook;
                var ws = wb.Worksheets[workSheetIndex];
                var cell = ws.Cells[rowNumber, columnNumber];
                var text = cell?.Text;
                return text;
            }
            catch (Exception e)
            {
                _log(e);
                throw;
            }
        }

        public int GetRowCount(int workSheetIndex)
        {
            try
            {
                var wb = _excelPackage.Workbook;
                var ws = wb.Worksheets[workSheetIndex];
                var nbRows = ws.Dimension.Rows;
                return nbRows;
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
