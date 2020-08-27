using System;
using System.Threading.Tasks;

namespace pvWay.MsSqlBackup
{
    public interface IMsSqlBackupCreator
    {
        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>IResult</returns>
        Task<IResult> BackupDbAsync(string bakFileName);

        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>IResult</returns>
        IResult BackupDb(string bakFileName);

        /// <summary>
        /// Creates a full backup of the database in background
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <param name="callback">A method that will be called on completion</param>
        /// <returns>void</returns>
        void BgBackupDb(string bakFileName, Action<IResult> callback);

        /// <summary>
        /// (1) finds the last backup file in the &lt;backupDestinationFolder&gt;
        /// by looking to files beginning with &lt;backupFilePrefix&gt; and ending with '.bak'
        /// (2) gets the last backup file modification time.
        /// (3) gets the elapsed time since this last backup.
        /// (4) if no file were found or elapsed time is greater
        /// than &lt;backupInterval&gt; then creates a new backup
        /// with a constructed file name
        /// &lt;backupFilePrefix&gt;_&lt;yyyyMMdd_HHmmss&gt;.bak;
        /// and save this new backup file into the &lt;backupDestinationFolder&gt;
        /// </summary>
        /// <param name="backupDestinationFolder"></param>
        /// <param name="backupFilePrefix"></param>
        /// <param name="backupInterval"></param>
        /// <returns>IExecuteMaintenancePlanResult</returns>
        Task<IExecuteMaintenancePlanResult> ExecuteMaintenancePlanAsync(
            string backupDestinationFolder,
            string backupFilePrefix,
            TimeSpan backupInterval);

        /// <summary>
        /// (1) finds the last backup file in the &lt;backupDestinationFolder&gt;
        /// by looking to files beginning with &lt;backupFilePrefix&gt; and ending with '.bak'
        /// (2) gets the last backup file modification time.
        /// (3) gets the elapsed time since this last backup.
        /// (4) if no file were found or elapsed time is greater
        /// than &lt;backupInterval&gt; then creates a new backup
        /// with a constructed file name
        /// &lt;backupFilePrefix&gt;_&lt;yyyyMMdd_HHmmss&gt;.bak;
        /// and save this new backup file into the &lt;backupDestinationFolder&gt;
        /// </summary>
        /// <param name="backupDestinationFolder">the folder where all backup files reside</param>
        /// <param name="backupFilePrefix"></param>
        /// <param name="backupInterval"></param>
        /// <returns>IExecuteMaintenancePlanResult</returns>
        IExecuteMaintenancePlanResult ExecuteMaintenancePlan(
            string backupDestinationFolder,
            string backupFilePrefix,
            TimeSpan backupInterval);

        /// <summary>
        /// Performs the following task in background and callbacks on completion:
        /// (1) finds the last backup file in the &lt;backupDestinationFolder&gt;
        /// by looking to files beginning with &lt;backupFilePrefix&gt; and ending with '.bak'
        /// (2) gets the last backup file modification time.
        /// (3) gets the elapsed time since this last backup.
        /// (4) if no file were found or elapsed time is greater
        /// than &lt;backupInterval&gt; then creates a new backup
        /// with a constructed file name
        /// &lt;backupFilePrefix&gt;_&lt;yyyyMMdd_HHmmss&gt;.bak;
        /// and save this new backup file into the &lt;backupDestinationFolder&gt;
        /// </summary>
        /// <param name="backupDestinationFolder"></param>
        /// <param name="backupFilePrefix"></param>
        /// <param name="backupInterval"></param>
        /// <param name="callback"></param>
        /// <returns>IExecuteMaintenancePlanResult</returns>
        void BgExecuteMaintenancePlan(
            string backupDestinationFolder,
            string backupFilePrefix,
            TimeSpan backupInterval,
            Action<IExecuteMaintenancePlanResult> callback);

    }

}