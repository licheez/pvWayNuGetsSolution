using pvWay.MethodResultWrapper.Core;
using pvWay.MsSqlLogWriter.Core;

namespace MsSqlLogWriterLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            const string cs = "data source=Localhost;initial catalog=iota700_dev;" +
                              "integrated security=True;MultipleActiveResultSets=True;";

            // let's start by creating the LogWriter
            // using all default values for table and columns names
            var msSqlLogWriter = new MsSqlLogWriter(cs);

            // let's now instantiate the PersistenceLogger class
            // from the pvWay.MethodResultWrapper.Core nuGet
            // The constructor for this class needs 3 params
            // (1) the dispose method of the logWriter
            // (2) a tuple delegate with named params for the WriteLog method
            // (3) a tuple delegate with named params for the WriteLogAsync method
            var persistenceLogger = new PersistenceLogger(
                msSqlLogWriter.Dispose,
                p => msSqlLogWriter.WriteLog(
                    p.userId, p.companyId, p.topic,
                    p.severityCode, p.machineName,
                    p.memberName, p.filePath, p.lineNumber,
                    p.message, p.dateUtc),
                async p => await msSqlLogWriter.WriteLogAsync(
                    p.userId, p.companyId, p.topic,
                    p.severityCode, p.machineName,
                    p.memberName, p.filePath, p.lineNumber,
                    p.message, p.dateUtc
                    ));
            
            // set some values for userId, companyId, and topic
            // so that the log columns will have some data
            persistenceLogger.SetUser("UserId", "CompanyId");
            persistenceLogger.SetTopic("Topic");

            // fire the pvWay.MethodResultWrapper PersistenceLogger.Log method
            // that will under the cover call the (sync) delegate
            // msSqlLogWriter.WriteLog method passed in the constructor
            // with the appropriate parameters
            persistenceLogger.Log("Hello Log");

            // this last line of code will generate an entry in the Log table
        }
    }
}
