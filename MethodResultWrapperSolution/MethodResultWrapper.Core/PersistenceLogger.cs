using System;
using System.Threading.Tasks;

namespace pvWay.MethodResultWrapper.Core
{
    public class PersistenceLogger : Logger
    {
        public PersistenceLogger(
            Action dispose,
            Action<(
                string userId, 
                string companyId, 
                string topic,
                string severityCode,
                string machineName, 
                string memberName,
                string filePath, 
                int lineNumber,
                string message, 
                DateTime dateUtc)> writeLog,
            Func<(
                string userId,
                string companyId,
                string topic,
                string severityCode,
                string machineName,
                string memberName,
                string filePath,
                int lineNumber,
                string message,
                DateTime dateUtc), Task> writeLogAsync) : 
            base(new PersistenceLogWriter(
                dispose, 
                writeLog,
                writeLogAsync))
        {
        }

        //private void Test()
        //{
        //    var cw = new ConsoleLogWriter();
        //    var pl = new PersistenceLogWriter(
        //        () =>
        //        {
        //            cw.Dispose();
        //        },
        //        p  =>
        //        {
        //            cw.WriteLog(
        //                p.userId, p.companyId, p.topic,
        //                p.severityCode, 
        //                p.machineName, p.memberName, 
        //                p.filePath, p.lineNumber,
        //                p.message, p.dateUtc);
        //        },
        //        async p =>
        //        {
        //            await cw.WriteLogAsync(
        //                p.userId, p.companyId, p.topic,
        //                p.severityCode,
        //                p.machineName, p.memberName,
        //                p.filePath, p.lineNumber,
        //                p.message, p.dateUtc);
        //        });
        //}

    }

}
