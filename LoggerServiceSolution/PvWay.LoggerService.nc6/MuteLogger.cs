namespace PvWay.LoggerService.nc6;

internal class MuteLogger : Logger, IPvWayMuteLoggerService
{
    public MuteLogger() : 
        base(new MuteLogWriter())
    {
    }
}