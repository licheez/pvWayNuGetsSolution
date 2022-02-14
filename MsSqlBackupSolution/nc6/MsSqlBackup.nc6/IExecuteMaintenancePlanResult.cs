namespace pvWay.MsSqlBackup.nc6;

public interface IExecuteMaintenancePlanResult : IResult
{
    bool BackupCreated { get; }
    string? BackupFileName { get; }
}