using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class PersistenceLogger : Logger
{
    public PersistenceLogger(
        Action dispose,
        Action<(
            string? userId, 
            string? companyId, 
            string? topic,
            SeverityEnum severity,
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
            SeverityEnum severity,
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