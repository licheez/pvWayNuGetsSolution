namespace pvWay.MethodResultWrapper.Model
{
    public class MuteLogger : Logger
    {
        public MuteLogger() : 
            base(new MuteLogWriter())
        {
        }
    }
}