namespace pvWay.MethodResultWrapper.nc6;

internal class PersistenceLogWriter : ILogWriter
{
    private readonly Action _dispose;
    private readonly Action<(
        string? userId,
        string? companyId,
        string? topic,
        string severityCode,
        string machineName,
        string memberName,
        string filePath,
        int lineNumber,
        string message,
        DateTime dateUtc)> _writeLog;

    private readonly Func<(
        string? userId,
        string? companyId,
        string? topic,
        string severityCode,
        string machineName,
        string memberName,
        string filePath,
        int lineNumber,
        string message,
        DateTime dateUtc), Task> _writeLogAsync;

    public PersistenceLogWriter(
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
            DateTime dateUtc), Task> writeLogAsync)
    {
        _dispose = dispose;
        _writeLog = writeLog;
        _writeLogAsync = writeLogAsync;
    }

    public void Dispose()
    {
        _dispose();
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        string severityCode,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        await _writeLogAsync(
            (userId, companyId, topic,
                severityCode,
                machineName, memberName,
                filePath, lineNumber,
                message, dateUtc));
    }

    public void WriteLog(
        string? userId, string? companyId, string? topic,
        string severityCode,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        _writeLog(
            (userId, companyId, topic,
                severityCode,
                machineName, memberName,
                filePath, lineNumber,
                message, dateUtc));
    }
}