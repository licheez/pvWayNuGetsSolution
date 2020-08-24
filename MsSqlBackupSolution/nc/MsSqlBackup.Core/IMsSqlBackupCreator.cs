using System.Threading.Tasks;
using pvWay.MethodResultWrapper.Core;

namespace pvWay.MsSqlBackup.Core
{
    public interface IMsSqlBackupCreator
    {
        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>MethodResult see pvWay MethodResultWrapper nuGet package</returns>
        Task<MethodResult> BackupDbAsync(string bakFileName);
    }
}