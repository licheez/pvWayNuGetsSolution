namespace pvWay.MethodResultWrapper.nc6
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() :
            base(new ConsoleLogWriter())
        {
        }
    }
}
