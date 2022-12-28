namespace pvWay.MsSqlSemaphore.nc6.Interfaces.Model;

public interface IDbSemaphore
{
    string Owner { get; }
    DateTime LastTouchUtcDate { get; }
}
