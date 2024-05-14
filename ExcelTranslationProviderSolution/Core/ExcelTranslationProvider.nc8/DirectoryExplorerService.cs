namespace ExcelTranslationProvider.nc8
{
    internal sealed class DirectoryExplorerService(Action<Exception> log)
    {
        public IEnumerable<string> GetMatchingFileNames(string folderPath, string fileSkeleton)
        {
            try
            {
                var di = new DirectoryInfo(folderPath);
                if (!di.Exists)
                {
                    throw new PvWayExcelTranslationProviderException(
                        $"folder {folderPath} does not exist");
                }
                return di.GetFiles(fileSkeleton)
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
