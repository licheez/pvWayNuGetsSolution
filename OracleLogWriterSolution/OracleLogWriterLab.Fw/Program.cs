using pvWay.OracleLogWriter.Fw;

namespace OracleLogWriterLab.Fw
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cs = "YOUR CONNECTION STRING";

            // let's start by creating the LogWriter
            // using all default values for table and columns names
            var oracleLogWriter = OracleLogWriterSingleton.GetInstance(cs);



        }
    }
}
