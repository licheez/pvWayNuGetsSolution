namespace pvWay.MethodResultWrapper.nc6;

public class ConsoleLogWriter: ILogWriter
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
        var line =
            $"severity: {severityCode}{Environment.NewLine}" +
            $"machineName: {machineName}{Environment.NewLine}" +
            $"memberName: {memberName}{Environment.NewLine}" +
            $"filePath: {filePath}{Environment.NewLine}" +
            $"lineNumber: {lineNumber}{Environment.NewLine}" +
            $"topic: {topic}{Environment.NewLine}" +
            $"message:{message}{Environment.NewLine}" +
            $"user: {userId}{Environment.NewLine}" +
            $"company: {companyId}{Environment.NewLine}" +
            $"dateUtc : {dateUtc}{Environment.NewLine}" +
            $"{Environment.NewLine}";
        Console.WriteLine(line);
    }
}