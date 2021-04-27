namespace pvWay.MethodResultWrapper.Model
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() :
            base(new ConsoleLogWriter())
        {
        }
    }
}
