using pvWay.MethodResultWrapper.Core;

namespace MethodResultWrapperLab.Core
{
    internal static class Program
    {
        private static void Main(/*string[] args*/)
        {
            var cw = new ConsoleLogWriter();
            var pw = new PersistenceLogger(
                () => cw.Dispose(),
                p =>
                    cw.WriteLog(
                        p.userId, p.companyId, p.topic,
                        p.severityCode,
                        p.machineName, p.memberName,
                        p.filePath, p.lineNumber,
                        p.message, p.dateUtc),
                async p =>
                    await cw.WriteLogAsync(
                        p.userId, p.companyId, p.topic,
                        p.severityCode,
                        p.machineName, p.memberName,
                        p.filePath, p.lineNumber,
                        p.message, p.dateUtc));
            pw.Log("test");
        }
    }
}
