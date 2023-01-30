namespace pvWay.MsSqlSemaphore.nc6.abstractions;

public enum DbSemaphoreStatusEnum
{
    Acquired,
    ReleasedInTheMeanTime,
    OwnedSomeoneElse,
    ForcedReleased
}