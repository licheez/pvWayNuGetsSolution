namespace PvWay.SemaphoreService.Abstractions.nc8;

public enum SemaphoreStatusEnu
{
    Acquired,
    ReleasedInTheMeanTime,
    OwnedSomeoneElse,
    ForcedReleased
}