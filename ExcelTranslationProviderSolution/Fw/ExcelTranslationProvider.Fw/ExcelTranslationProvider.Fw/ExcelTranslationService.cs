using System;
using System.Collections.Generic;
using System.Linq;

namespace pvWay.ExcelTranslationProvider.Fw
{
    public class ExcelTranslationService : IExcelTranslationService
    {
        private readonly Action<Exception> _log;
        private readonly string _excelTranslationsFolder;
        private readonly string _excelTranslationsFileSkeleton;
        private readonly DirectoryExplorerService _des;

        public ExcelTranslationService(
            Action<Exception> log,
            string excelTranslationsFolder,
            string excelTranslationsFileSkeleton)
        {
            _log = log;
            _excelTranslationsFolder = excelTranslationsFolder;
            _excelTranslationsFileSkeleton = excelTranslationsFileSkeleton;
            _des = new DirectoryExplorerService(log);
        }

        public DateTime LastUpdateDateUtc
        {
            get
            {
                try
                {
                    return _des.GetMostRecentFileModificationDateUtc(
                        _excelTranslationsFolder,
                        _excelTranslationsFileSkeleton);
                }
                catch (Exception e)
                {
                    _log(e);
                    throw;
                }
            }
        }

        public IDictionary<string, IDictionary<string, string>> Translations
        {
            get
            {
                try
                {
                    var excelFileNames = _des.GetMatchingFileNames(
                        _excelTranslationsFolder, _excelTranslationsFileSkeleton);
                    var rows = new List<ExcelDataRow>();
                    foreach (var excelFileName in excelFileNames)
                    {
                        using (var xReader = new ExcelReader(_log, excelFileName))
                        {
                            var nbRows = xReader.RowCount;
                            var header = new ExcelHeaderRow(xReader);
                            // start at row one to skip the header
                            for (var r = 1; r <= nbRows; r++)
                            {
                                var row = new ExcelDataRow(xReader, header.LangMap, r);
                                rows.Add(row);
                            }
                        }
                    }
                    return rows.ToDictionary(k => k.Key, v => v.Translations);
                }
                catch (Exception e)
                {
                    _log(e);
                    throw;
                }
            }
        }

        private class ExcelHeaderRow
        {
            public readonly IDictionary<string, int> LangMap;

            public ExcelHeaderRow(ExcelReader er)
            {
                LangMap = new Dictionary<string, int>();
                var col = 4;
                while (true)
                {
                    var langCode = er.GetCellText(0, col);
                    if (string.IsNullOrEmpty(langCode)) break;
                    LangMap.Add(langCode, col);
                    col++;
                }
            }
        }

        private class ExcelDataRow
        {
            public string Key { get; }

            public IDictionary<string, string> Translations { get; }

            public ExcelDataRow(
                ExcelReader er,
                IDictionary<string, int> langMap,
                int row)
            {
                var kParts = new[]
                {
                    er.TabName,
                    er.GetCellText(row, 0),
                    er.GetCellText(row, 1),
                    er.GetCellText(row, 2),
                    er.GetCellText(row, 3)
                };
                Key = string.Empty;
                foreach (var kPart in kParts)
                {
                    if (string.IsNullOrEmpty(kPart)) break;
                    if (!string.IsNullOrEmpty(Key)) Key += ".";
                    Key += kPart;
                }

                Translations = new Dictionary<string, string>();
                foreach (var kvp in langMap)
                {
                    var translation = er.GetCellText(row, kvp.Value);
                    Translations.Add(kvp.Key, translation);
                }
            }

            public override string ToString()
            {
                var str = $"{Key}:";
                foreach (var kvp in Translations)
                {
                    str += $"{kvp.Key}-{kvp.Value} ";
                }
                return str;
            }
        }

        public void Dispose()
        {
            // nop
        }
    }
}
