namespace pvWay.ViesApi.nc6;

internal class ViesResult : IViesResult
{
    public bool Success { get; }
    public bool Failure => !Success;
    public Exception? Exception { get; }
    public IViesData? Data { get; }

    public ViesResult(IViesData data)
    {
        Success = true;
        Exception = null;
        Data = data;
    }

    public ViesResult(Exception e)
    {
        Success = false;
        Exception = e;
        Data = null;
    }
}