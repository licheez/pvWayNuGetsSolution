using PvWay.ExcelTranslationProvider.nc8.Exceptions;

namespace PvWay.ExcelTranslationProvider.nc8.Helpers
{
    internal sealed class DirectoryExplorerService(Action<Exception> log)
    {
        public IEnumerable<string> GetMatchingFileNames(string folderPath, string fileWildCard)
        {
            try
            {
                var di = new DirectoryInfo(folderPath);
                if (!di.Exists)
                {
                    throw new PvWayExcelTranslationProviderException(
                        $"folder {folderPath} does not exist");
                }
                return di.GetFiles(fileWildCard)
                    .Select(x => x.FullName);
            }
            catch (Exception e)
            {
                log(e);
                throw;
            }
        }

        public DateTime GetMostRecentFileModificationDateUtc(string folderPath, string fileSkeleton)
        {
            try
            {
                var fileNames = GetMatchingFileNames(folderPath, fileSkeleton);
                var writeTimeUtc = DateTime.MinValue;
                foreach (var fileName in fileNames)
                {
                    var fi = new FileInfo(fileName);
                    if (fi.LastWriteTimeUtc > writeTimeUtc)
                        writeTimeUtc = fi.LastWriteTimeUtc;
                }

                return writeTimeUtc;
            }
            catch (Exception e)
            {
                log(e);
                throw;
            }
        }

    }
}
