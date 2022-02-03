namespace pvWay.MethodResultWrapper.nc6;

public class PersistenceLogger : Logger
{
    public PersistenceLogger(
        Action dispose,
        Action<(
            string? userId, 
            string? companyId, 
            string? topic,
            string severityCode,
            string machineName, 
            string memberName,
            string filePath, 
            int lineNumber,
            string message, 
            DateTime dateUtc)> writeLog,
        Func<(
            string? userId,
            string? companyId,
            string? topic,
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

}