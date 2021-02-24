using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pvWay.ExcelTranslationProvider.Core
{
    internal class DirectoryExplorerService 
    {
        private readonly Action<Exception> _log;

        public DirectoryExplorerService(Action<Exception> log)
        {
            _log = log;
        }

        public IEnumerable<string> GetMatchingFileNames(string folderPath, string fileSkeleton)
        {
            try
            {
                var di = new DirectoryInfo(folderPath);
                if (!di.Exists)
                {
                    throw new Exception($"folder {folderPath} does not exist");
                }

                return di.GetFiles(fileSkeleton)
                    .Select(x => x.FullName);
            }
            catch (Exception e)
            {
                _log(e);
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
                _log(e);
                throw;
            }
        }

    }
}
