namespace pvWay.MethodResultWrapper.nc6;

internal class MuteLogWriter : ILogWriter
{
    public void Dispose()
    {
        // nop
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, string? topic,
        string severityCode, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        await Task.Run(() =>
        {
            WriteLog(userId, companyId, topic,
                severityCode, machineName, memberName,
                filePath, lineNumber, message, dateUtc);
        });
    }

    public void WriteLog(
        string? userId, string? companyId, string? topic,
        string severityCode, string machineName,
        string memberName, string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        // nop
    }
}