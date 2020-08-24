# Ms Sql Backup

Tiny DAO utility that backs up any Ms Sql Db (any edition) to a local or network drive.

## Interface

The nuGet has only one method

```csharp
using System.Threading.Tasks;
using pvWay.MethodResultWrapper.Model;

namespace pvWay.MsSqlBackup
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
```

## Usage

See here after a short Console that use the service

### Principe

* need to pass a work folder where the Ms Sql Server has write access
* need to pass the connection string
* need to pass a destination file where the app has write access
* you can follow up on progress by passing a notifier callback

remark: you'll need to get the [MethodResultWrapper](https://github.com/licheez/pvWayNuGetsSolution/tree/master/MethodResultWrapperSolution/MethodResultWrapper) to use this service

### The code

```csharp

using System;
using pvWay.MethodResultWrapper.Model;
using pvWay.MsSqlBackup;

namespace MsSqlBackupLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            // need to pass the ILoggerService from pvWay MethodResultWrapper nuGet package
            // her I use the ConsoleLogger implementation
            // see https://github.com/licheez/pvWayNuGetsSolution/tree/master/MethodResultWrapperSolution/MethodResultWrapper
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