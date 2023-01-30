namespace pvWay.MsSqlSemaphore.nc6.abstractions;

public interface IDbSemaphore
{
    string Owner { get; }
    DateTime LastTouchUtcDate { get; }
}
