namespace pvWay.MsSqlBackup.nc6;

internal class Result:IResult
{
    public bool Success { get; }
    public bool Failure => !Success;
    public Exception? Exception { get; }

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