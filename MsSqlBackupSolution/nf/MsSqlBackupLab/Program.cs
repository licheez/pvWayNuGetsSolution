using System;
using pvWay.MsSqlBackup;

namespace MsSqlBackupLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            // make sure Ms Sql Server has write access to this work folder
            const string localWorkFolder = "d:\\temp";

            // the connection string to the db you want to back up
            const string connectionString = "data source = localhost; " +
                                            "initial catalog = TCM004_DEV; " +
                                            "integrated security = True; " +
                                            "MultipleActiveResultSets = True; ";

            var backupCreator = new MsSqlBackupCreator(
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
                Console.WriteLine(res.Exception);
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
