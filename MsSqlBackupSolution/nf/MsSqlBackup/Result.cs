using System;

namespace pvWay.MsSqlBackup
{
    internal class Result:IResult
    {
        public bool Success { get; }
        public bool Failure => !Success;
        public Exception Exception { get; }

        protected Result()
        {
            Success = true;
        }

        public Result(Exception e)
        {
            Success = false;
            Exception = e;
        }

        public static IResult Ok => new Result();
    }

    internal class ExecuteMaintenancePlanResult : Result, IExecuteMaintenancePlanResult
    {
        public bool BackupCreated { get; }
        public string BackupFileName { get; }

        public ExecuteMaintenancePlanResult(
            string backupFileName = null)
        {
            BackupCreated = !string.IsNullOrEmpty(backupFileName);
            BackupFileName = backupFileName;
        }
        
        public ExecuteMaintenancePlanResult(Exception e): base(e)
        {
            BackupCreated = false;
        }
    }
}
