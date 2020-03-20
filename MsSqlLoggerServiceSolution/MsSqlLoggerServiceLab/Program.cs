using System;
using pvWay.MethodResultWrapper;
using pvWay.MsSqlLoggerService;

namespace MsSqlLoggerServiceLab
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cn = "data source=Localhost;initial catalog=iota_PRD_20200208;" +
                              "integrated security=True;MultipleActiveResultSets=True;";

            var ls = new Logger(
                cn,
                SeverityEnum.Debug,
                "dbo",
                "Log",
                "UserId",
                "CompanyId",
                "MachineName",
                "SeverityCode",
                "Context",
                "Topic",
                "Message",
                "CreateDateUtc");

            ls.Log(new Exception());

            Console.WriteLine("hit enter to quit");
            Console.ReadLine();
        }
    }
}
