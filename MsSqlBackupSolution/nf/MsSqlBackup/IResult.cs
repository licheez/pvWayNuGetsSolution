using System;

namespace pvWay.MsSqlBackup
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