namespace pvWay.MethodResultWrapper.Core
{
    public class MuteLogger : Logger
    {
        public MuteLogger() : 
            base(new MuteLogWriter())
        {
        }
    }
}