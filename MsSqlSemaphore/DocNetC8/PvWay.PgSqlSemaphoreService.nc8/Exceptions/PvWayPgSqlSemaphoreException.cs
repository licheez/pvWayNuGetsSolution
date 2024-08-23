namespace PvWay.PgSqlSemaphoreService.nc8.Exceptions;

public class PvWayPgSqlSemaphoreException: Exception
{
    public PvWayPgSqlSemaphoreException(string message) :
        base($"PvWayPgSqlSemaphoreException:{message}"){}

    public PvWayPgSqlSemaphoreException(Exception e): 
        base($"PvWayPgSqlSemaphoreException:{e.Message}", e)
    {
    }
    
}