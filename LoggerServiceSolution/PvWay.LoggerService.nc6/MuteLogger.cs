namespace PvWay.LoggerService.nc6
{
    public class MuteLogger : Logger
    {
        public MuteLogger() : 
            base(new MuteLogWriter())
        {
        }
    }
}