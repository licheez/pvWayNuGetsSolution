namespace pvWay.MsSqlBackup.nc6;

public interface IResult
{
    bool Success { get; }
    bool Failure { get; }
    Exception? Exception { get; }
}