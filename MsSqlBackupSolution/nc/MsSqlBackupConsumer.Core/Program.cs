using System;
using pvWay.MsSqlBackup.Core;

namespace MsSqlBackupConsumer.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            // make sure Ms Sql Server has write access to this work folder
            const string localWorkFolder = "d:\\temp";

            const string bakDestFolder = "d:\\msSqlBak";
            var bakInterval = TimeSpan.FromDays(1);
            const string bakPrefix = "tcm_";

            // the connection string to the db you want to back up
            const string connectionString = "data source = localhost; " +
                                            "initial catalog = TCM004_DEV; " +
                                            "integrated security = True; " +
                                            "MultipleActiveResultSets = True; ";

            IMsSqlBackupCreator backupCreator = new MsSqlBackupCreator(
                localWorkFolder,
                connectionString,
                // let's redirect progress notifications to the console
                Console.WriteLine);

            // ok now launch an maintenance plan execution
            var res = backupCreator
                .ExecuteMaintenancePlan(
                    bakDestFolder,
                    bakPrefix,
                    bakInterval);

            /*
            
            CONSOLE Should display this
            ---------------------------
            nuGet pvWay.MsSqlBackup connecting to localhost
            nuGet pvWay.MsSqlBackup backing up database TCM004_DEV to local work folder d:\temp\
            nuGet pvWay.MsSqlBackup copying backup file to destination: d:\msSqlBaktcm_20200827_102503.bak
            nuGet pvWay.MsSqlBackup removing temp file from d:\temp\

             */

            if (res.Failure)
            {
                Console.WriteLine(res.Exception);
            }
            else
            {
                Console.WriteLine(
                    res.BackupCreated
                    ? $"Backup file {res.BackupFileName} created"
                    : "No backup needed at this time");
            }

            Console.WriteLine("hit a key to quit");
            Console.ReadKey();

        }
    }
}
