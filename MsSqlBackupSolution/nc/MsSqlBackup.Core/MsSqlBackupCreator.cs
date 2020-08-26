using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using pvWay.MethodResultWrapper.Core;

namespace pvWay.MsSqlBackup.Core
{
    public class MsSqlBackupCreator : IMsSqlBackupCreator
    {
        private readonly ILoggerService _ls;
        private readonly string _localWorkFolder;
        private readonly string _connectionString;
        private readonly Action<string> _notify;
        private readonly string _dataSource;
        private readonly string _catalog;

        private const string Notifier = "nuGet pvWay.MsSqlBackup";

        /// <summary>
        /// construct with (1) nuGet package pvWay MethodResultWrapper ILoggerService,
        /// (2) a work folder where the Ms Sql Server is allowed to write,
        /// (3) the connection string to the Db to backup and
        /// (4) an optional callback that will get progress notifications
        /// </summary>
        /// <param name="ls">nuGet package pvWay MethodResultWrapper ILoggerService</param>
        /// <param name="localWorkFolder">a place where the Ms Sql Server is allowed to write</param>
        /// <param name="connectionString">the connection string to the Db to backup</param>
        /// <param name="notify">optional callback that will get progress notifications</param>
        public MsSqlBackupCreator(
            ILoggerService ls,
            string localWorkFolder,
            string connectionString,
            Action<string> notify = null)
        {
            _ls = ls;
            _localWorkFolder = localWorkFolder.EndsWith("\\")
                ? localWorkFolder : localWorkFolder + "\\";
            _connectionString = connectionString;
            _notify = notify;
            var cs = new SqlConnectionStringBuilder(connectionString);
            _dataSource = cs.DataSource;
            _catalog = cs.InitialCatalog;
        }

        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>MethodResult see pvWay MethodResultWrapper nuGet package</returns>
        public IMethodResult BackupDb(string bakFileName)
        {
            var tmpFileName = $"{_localWorkFolder}tmp_{Guid.NewGuid()}.bak";
            try
            {
                using var cn = new SqlConnection(_connectionString);
                _notify?.Invoke($"{Notifier} connecting to {_dataSource}");
                cn.Open();
                using var cmd = cn.CreateCommand();
                cmd.CommandText = $"BACKUP DATABASE [{_catalog}] TO DISK='{tmpFileName}'";
                cmd.CommandType = CommandType.Text;

                _notify?.Invoke($"{Notifier} backing up database {_catalog} to local work folder {_localWorkFolder}");
                cmd.ExecuteNonQuery();

                _notify?.Invoke($"{Notifier} copying backup file to destination: {bakFileName}");
                File.Copy(tmpFileName, bakFileName);

                cn.Close();

                return MethodResult.Ok;
            }
            catch (Exception e)
            {
                _ls.Log(e);
                return new MethodResult(e);
            }
            finally
            {
                if (File.Exists(tmpFileName))
                {
                    _notify?.Invoke($"{Notifier} removing temp file from {_localWorkFolder}");

                    File.Delete(tmpFileName);
                }
            }
        }

        /// <summary>
        /// Creates a full backup of the database in background
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <param name="callback">A method that will be called on completion</param>
        /// <returns>void</returns>
        public void BgBackupDb(string bakFileName, Action<IMethodResult> callback)
        {
            var bag = new Bag(bakFileName, callback);
            var asyncWorker = new Thread(BackupUpDbThread);
            asyncWorker.Start(bag);
        }

        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
        /// <returns>MethodResult see pvWay MethodResultWrapper nuGet package</returns>
        public async Task<IMethodResult> BackupDbAsync(string bakFileName)
        {
            var tmpFileName = $"{_localWorkFolder}tmp_{Guid.NewGuid()}.bak";
            try
            {
                await using var cn = new SqlConnection(_connectionString);
                _notify?.Invoke($"{Notifier} connecting to {_dataSource}");
                await cn.OpenAsync();
                await using var cmd = cn.CreateCommand();
                cmd.CommandText = $"BACKUP DATABASE [{_catalog}] TO DISK='{tmpFileName}'";
                cmd.CommandType = CommandType.Text;

                _notify?.Invoke($"{Notifier} backing up database {_catalog} to local work folder {_localWorkFolder}");
                cmd.ExecuteNonQuery();

                _notify?.Invoke($"{Notifier} copying backup file to destination: {bakFileName}");
                File.Copy(tmpFileName, bakFileName);

                cn.Close();

                return MethodResult.Ok;
            }
            catch (Exception e)
            {
                _ls.Log(e);
                return new MethodResult(e);
            }
            finally
            {
                if (File.Exists(tmpFileName))
                {
                    _notify?.Invoke($"{Notifier} removing temp file from {_localWorkFolder}");

                    File.Delete(tmpFileName);
                }
            }
        }

        private class Bag
        {
            public string BakFileName { get; }
            public Action<IMethodResult> Callback { get; }

            public Bag(
                string bakFileName,
                Action<IMethodResult> callback)
            {
                BakFileName = bakFileName;
                Callback = callback;
            }
        }

        private void BackupUpDbThread(object data)
        {
            var bag = (Bag)data;
            var res = BackupDb(bag.BakFileName);
            bag.Callback?.Invoke(res);
        }
    }
}
