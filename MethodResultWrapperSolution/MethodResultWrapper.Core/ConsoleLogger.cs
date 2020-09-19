// ReSharper disable ExplicitCallerInfoArgument
namespace pvWay.MethodResultWrapper.Core
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() :
            base(new ConsoleLogWriter())
        {
        }
    }
}
