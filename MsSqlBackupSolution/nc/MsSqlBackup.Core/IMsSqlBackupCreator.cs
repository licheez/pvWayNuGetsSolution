using System;
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
        Task<IMethodResult> BackupDbAsync(string bakFileName);

        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>MethodResult see pvWay MethodResultWrapper nuGet package</returns>
        IMethodResult BackupDb(string bakFileName);

        /// <summary>
        /// Creates a full backup of the database in background
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <param name="callback">A method that will be called on completion</param>
        /// <returns>void</returns>
        void BgBackupDb(string bakFileName, Action<IMethodResult> callback);
    }
}