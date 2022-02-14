using System.Data;
using System.Data.SqlClient;
using Azure.Storage.Blobs;

namespace pvWay.MsSqlBackup.nc6;

public class MsSqlBackupCreator : IMsSqlBackupCreator
{
    private readonly string _localWorkFolder;
    private readonly string _connectionString;
    private readonly Action<string>? _notify;
    private readonly string _dataSource;
    private readonly string _catalog;

    private const string Notifier = "nuGet pvWay.MsSqlBackup.nc6";

    /// <summary>
    /// construct with (1) nuGet package pvWay MethodResultWrapper ILoggerService,
    /// (2) a work folder where the Ms Sql Server is allowed to write,
    /// (3) the connection string to the Db to backup and
    /// (4) an optional callback that will get progress notifications
    /// </summary>
    /// <param name="localWorkFolder">a place where the Ms Sql Server is allowed to write</param>
    /// <param name="connectionString">the connection string to the Db to backup</param>
    /// <param name="notify">optional callback that will get progress notifications</param>
    public MsSqlBackupCreator(
        string localWorkFolder,
        string connectionString,
        Action<string>? notify = null)
    {
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
    public async Task<IResult> BackupDbAsync(string bakFileName)
    {
        return await BackupDbAsync(
            tmpFileName =>
            {
                _notify?.Invoke($"{Notifier} copying backup file to destination: {bakFileName}");
                File.Copy(tmpFileName, bakFileName);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Creates a full backup of the database
    /// </summary>
    /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
    /// <returns>IResult</returns>
    public IResult BackupDb(string bakFileName)
    {
        return BackupDbAsync(bakFileName).Result;
    }

    /// <summary>
    /// Creates a full backup of the database in background
    /// </summary>
    /// <param name="bakFileName">Fully qualified backup file name where the running app has write access</param>
    /// <param name="callback">A method that will be called on completion</param>
    /// <returns>void</returns>
    public void BgBackupDb(string bakFileName, Action<IResult>? callback)
    {
        var bag = new BackupBag(bakFileName, callback);
        var asyncWorker = new Thread(BackupUpDbThread);
        asyncWorker.Start(bag);
    }

    public async Task<IResult> BackupDbAsync(
        string azureStorageConnectionString, string containerName, string blobName)
    {
        return await BackupDbAsync(
            async tmpFileName =>
            {
                var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
                if (blobServiceClient == null)
                    throw new Exception($"{Notifier} unable to instantiate the BlobServiceClient");
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                if (blobContainerClient == null)
                {
                    _notify?.Invoke($"{Notifier} creating azure container {containerName}");
                    var createContainerClient = await blobServiceClient
                        .CreateBlobContainerAsync(containerName);
                    blobContainerClient = createContainerClient.Value;
                }
                _notify?.Invoke($"{Notifier} uploading backup file to azure container: {containerName}/{blobName}");
                var blobClient = blobContainerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(tmpFileName, true);
                _notify?.Invoke($"{Notifier} file uploaded");
            });
    }

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
    public async Task<IExecuteMaintenancePlanResult> ExecuteMaintenancePlanAsync(
        string backupDestinationFolder,
        string backupFilePrefix,
        TimeSpan backupInterval)
    {

        try
        {
            // find last backup file
            // get last file modification time
            // get elapsed time since last backup
            // if elapsed time is greater than backupInterval create a new backup

            if (!backupDestinationFolder.EndsWith("\\"))
                backupDestinationFolder += "\\";

            // verify that the backupDestination folder exists
            var backupDi = new DirectoryInfo(backupDestinationFolder);
            if (!backupDi.Exists)
            {
                var err = $"Folder {backupDestinationFolder} does not exist";
                _notify?.Invoke(err);
                var e = new Exception(err);
                return new ExecuteMaintenancePlanResult(e);
            }

            // find last backup file
            var searchPattern = $"{backupFilePrefix}*.bak";
            var backupFiles = backupDi.GetFiles(searchPattern)
                .OrderByDescending(x => x.CreationTimeUtc);
            var lastFile = backupFiles.FirstOrDefault();

            // get last file modification time
            var lastFileCreationTimeUtc = lastFile?.CreationTimeUtc ?? DateTime.MinValue;
            var now = DateTime.UtcNow;

            // get elapsed time since last backup
            var elapsedTimeSinceLastBackup = now - lastFileCreationTimeUtc;

            // if elapsed time is greater than backupInterval create a new backup
            if (elapsedTimeSinceLastBackup < backupInterval)
                return new ExecuteMaintenancePlanResult();

            // create a backup
            var bakFilePath = $"{backupDestinationFolder}{backupFilePrefix}{now:yyyyMMdd_HHmmss}.bak";
            var res = await BackupDbAsync(bakFilePath);
            if (res.Success) return new ExecuteMaintenancePlanResult(bakFilePath);

            _notify?.Invoke($"{Notifier} failed {res.Exception}");
            return new ExecuteMaintenancePlanResult(res.Exception!);
        }
        catch (Exception e)
        {
            return new ExecuteMaintenancePlanResult(e);
        }
    }

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
    public IExecuteMaintenancePlanResult ExecuteMaintenancePlan(
        string backupDestinationFolder,
        string backupFilePrefix,
        TimeSpan backupInterval)
    {
        return ExecuteMaintenancePlanAsync(
            backupDestinationFolder,
            backupFilePrefix,
            backupInterval).Result;
    }

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
    public void BgExecuteMaintenancePlan(
        string backupDestinationFolder,
        string backupFilePrefix,
        TimeSpan backupInterval,
        Action<IExecuteMaintenancePlanResult>? callback)
    {
        var bag = new PlanBag(
            backupDestinationFolder,
            backupFilePrefix,
            backupInterval,
            callback);
        var asyncWorker = new Thread(ExecuteMaintenancePlanThread);
        asyncWorker.Start(bag);
    }

    private class BackupBag
    {
        public string BakFileName { get; }
        public Action<IResult>? Callback { get; }

        public BackupBag(
            string bakFileName,
            Action<IResult>? callback)
        {
            BakFileName = bakFileName;
            Callback = callback;
        }
    }

    private void BackupUpDbThread(object? data)
    {
        if (data == null)
            throw new Exception("data should not be null");
        var bag = (BackupBag)data;
        var res = BackupDb(bag.BakFileName);
        bag.Callback?.Invoke(res);
    }

    private class PlanBag
    {
        public string BackupDestinationFolder { get; }
        public string BackupFilePrefix { get; }
        public TimeSpan BackupInterval { get; }
        public Action<IExecuteMaintenancePlanResult>? Callback { get; }

        public PlanBag(
            string backupDestinationFolder,
            string backupFilePrefix,
            TimeSpan backupInterval,
            Action<IExecuteMaintenancePlanResult>? callback)
        {
            BackupDestinationFolder = backupDestinationFolder;
            BackupFilePrefix = backupFilePrefix;
            BackupInterval = backupInterval;
            Callback = callback;
        }
    }

    private void ExecuteMaintenancePlanThread(object? data)
    {
        if (data == null)
            throw new Exception("data should not be null");
        var bag = (PlanBag)(data);
        var res = ExecuteMaintenancePlan(
            bag.BackupDestinationFolder,
            bag.BackupFilePrefix,
            bag.BackupInterval);
        bag.Callback?.Invoke(res);
    }

    private async Task<IResult> BackupDbAsync(Func<string, Task> copyFile)
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

            await copyFile(tmpFileName);

            cn.Close();

            return Result.Ok;
        }
        catch (Exception e)
        {
            _notify?.Invoke($"{Notifier} raised an exception {e}");
            return new Result(e);
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

}