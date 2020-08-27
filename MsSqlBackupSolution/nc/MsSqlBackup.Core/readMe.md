# Ms Sql Backup dotNet Core

Tiny DAO utility that backs up any Ms Sql Db (any edition) to a local or network drive and execute maintenance plans

## Interfaces

### IMsSqlBackupCreator

The nuGet has several methods all in three modes sync, async and pure background.

```csharp
using System;
using System.Threading.Tasks;

namespace pvWay.MsSqlBackup.Core
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
```

### return values

```csharp
using System;

namespace pvWay.MsSqlBackup.Core
{
    public interface IResult
    {
        bool Success { get; }
        bool Failure { get; }
        Exception Exception { get; }
    }

    public interface IExecuteMaintenancePlanResult : IResult
    {
        bool BackupCreated { get; }
        string BackupFileName { get; }
    }

}
```

## Usage

See here after a short Console that use the service

### Principe

* need to pass a work folder where the Ms Sql Server has write access
* need to pass the connection string
* need to pass a destination file where the app has write access
* you can follow up on progress by passing a notifier callback

### The code

```csharp

using System;
using pvWay.MethodResultWrapper.Model;
using pvWay.MsSqlBackup.Core;

namespace MsSqlBackupLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            // need to pass the ILoggerService from pvWay MethodResultWrapper nuGet package
            // her I use the ConsoleLogger implementation
            // see remark: you'll need to get the [MethodResultWrapper](https://github.com/licheez/pvWayNuGetsSolution/tree/master/MethodResultWrapperSolution/MethodResultWrapper) to use this service
            var ls = new ConsoleLogger();

            // make sure Ms Sql Server has write access to this work folder
            const string localWorkFolder = "d:\\temp";

            // the connection string to the db you want to back up
            const string connectionString = "data source = localhost; " +
                                            "initial catalog = TCM004_DEV; " +
                                            "integrated security = True; " +
                                            "MultipleActiveResultSets = True; ";

            var backupCreator = new MsSqlBackupCreator(
                ls,
                localWorkFolder,
                connectionString,
                // let's redirect progress notifications to the console
                Console.WriteLine);

            // let's generate a unique bak file name using a GUID
            var id = Guid.NewGuid().ToString();
            var bakFileNameNwDrive = $"y:\\MsSqlBak\\Tcm004_{id}.bak";

            // ok now launch the backup
            var res = backupCreator
                .BackupDbAsync(bakFileNameNwDrive).Result;

            /*
            
            CONSOLE Should display this
            ---------------------------
            nuGet pvWay.MsSqlBackup connecting to localhost
            nuGet pvWay.MsSqlBackup backing up database TCM004_DEV to local work folder d:\temp\
            nuGet pvWay.MsSqlBackup copying backup file to destination: y:\MsSqlBak\Tcm004_85daa4de-d2b3-42e8-99bb-aad2252f1b41.bak
            nuGet pvWay.MsSqlBackup removing temp file from d:\temp

            */

            if (res.Failure)
            {
                ls.Log(res);
            }
            else
            {
                Console.WriteLine("Backup completed");
            }

            Console.WriteLine("hit a key to quit");
            Console.ReadKey();
        }
    }
}

```

Happy coding