namespace pvWay.MsSqlSemaphore.nc6.Interfaces.Enums;

public enum DbSemaphoreStatusEnum
{
    Acquired,
    ReleasedInTheMeanTime,
    OwnedSomeoneElse,
    ForcedReleased
}