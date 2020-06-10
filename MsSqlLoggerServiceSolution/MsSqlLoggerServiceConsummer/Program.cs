using System;
using pvWay.MethodResultWrapper;

namespace MsSqlLoggerServiceConsumer
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cn = "data source=Localhost;initial catalog=iota_PRD_20200208;" +
                              "integrated security=True;MultipleActiveResultSets=True;";

            var ls = new LoggerService(
                cn,
                SeverityEnum.Debug,
                "dbo",
                "Log",
                "UserId",
                "CompanyId",
                "MachineName",
                "SeverityCode",
                "Topic",
                "Context",
                "Message",
                "CreateDateUtc");

            ls.Log(new Exception());

            Console.WriteLine("hit enter to quit");
            Console.ReadLine();

        }
    }
}
