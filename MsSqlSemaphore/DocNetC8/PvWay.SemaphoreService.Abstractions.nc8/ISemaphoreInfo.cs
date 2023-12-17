namespace PvWay.SemaphoreService.Abstractions.nc8;

public interface ISemaphoreInfo
{
    SemaphoreStatusEnu Status { get; }
    
    string Owner { get; }
    TimeSpan Timeout { get; }
    DateTime ExpiresAtUtc { get; }
    DateTime CreateDateUtc { get; }
    DateTime UpdateUtcDate { get; }
}
