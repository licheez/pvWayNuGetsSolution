using pvWay.MsSqlSemaphore.nc6.Interfaces.Model;

namespace pvWay.MsSqlSemaphore.nc6.Model;

internal record DbSemaphore (
    string Owner, 
    DateTime LastTouchUtcDate): IDbSemaphore;