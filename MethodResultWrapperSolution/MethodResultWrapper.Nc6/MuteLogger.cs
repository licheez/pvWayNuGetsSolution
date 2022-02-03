namespace pvWay.MethodResultWrapper.nc6
{
    public class MuteLogger : Logger
    {
        public MuteLogger() : 
            base(new MuteLogWriter())
        {
        }
    }
}