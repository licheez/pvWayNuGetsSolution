using pvWay.MsSqlSemaphore.nc6.abstractions;

namespace pvWay.MsSqlSemaphore.nc6.Model;

internal record DbSemaphore (
    string Owner, 
    DateTime LastTouchUtcDate): IDbSemaphore;