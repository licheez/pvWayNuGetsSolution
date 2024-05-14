using PvWay.ExcelTranslationProvider.Abstractions.nc8;
using PvWay.ExcelTranslationProvider.nc8.Helpers;
using PvWay.ExcelTranslationProvider.nc8.Model;

namespace PvWay.ExcelTranslationProvider.nc8.Services
{
    internal sealed class PvWayFolderBasedExcelTranslationService(
        IPvWayFolderBasedExcelTranslationServiceConfig config)
        : IPvWayExcelTranslationService
    {
        private readonly Action<Exception> _logException = config.LogException;
        private readonly string _excelTranslationsFolder = config.ExcelFolderPath;
        private readonly string _excelTranslationsFileSkeleton = config.FileWildCard;

        private readonly DirectoryExplorerService _des = new (config.LogException);

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
                    _logException(e);
                    throw;
                }
            }
        }

        public IDictionary<string, IDictionary<string, string>> ReadTranslations()
        {
            try
            {
                var excelFileNames = _des.GetMatchingFileNames(
                    _excelTranslationsFolder, _excelTranslationsFileSkeleton);
                var rows = new List<ExcelDataRow>();
                foreach (var excelFileName in excelFileNames)
                {
                    using var xReader = new FolderBasedExcelReader(
                        _logException, excelFileName);
                    var nbRows = xReader.RowCount;
                    var header = new ExcelHeaderRow(xReader);
                    // start at row one to skip the header
                    for (var r = 1; r < nbRows; r++)
                    {
                        var row = new ExcelDataRow(xReader, header.LangMap, r);
                        rows.Add(row);
                    }
                }
                return rows.ToDictionary(
                    k => k.Key, 
                    v => v.Translations);
            }
            catch (Exception e)
            {
                _logException(e);
                throw;
            }
        }


        public void Dispose()
        {
            // no operation
        }
    }
}
