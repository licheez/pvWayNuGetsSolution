namespace pvWay.MsSqlBackup.nc6;

internal class ExecuteMaintenancePlanResult : Result, IExecuteMaintenancePlanResult
{
    public bool BackupCreated { get; }
    public string? BackupFileName { get; }

    public ExecuteMaintenancePlanResult(
        string? backupFileName = null)
    {
        BackupCreated = !string.IsNullOrEmpty(backupFileName);
        BackupFileName = backupFileName;
    }
        
    public ExecuteMaintenancePlanResult(Exception e): base(e)
    {
        BackupCreated = false;
    }
}