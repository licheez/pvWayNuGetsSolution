namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface ISemaphoreInfo
{
    string Owner { get; }
    DateTime LastTouchUtcDate { get; }
}
