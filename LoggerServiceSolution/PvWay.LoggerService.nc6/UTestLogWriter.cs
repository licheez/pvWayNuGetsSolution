using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class UTestLogWriter: IPvWayUTestLogWriter
{
    private int _lastId;
    private readonly IList<LogRow> _rows = new List<LogRow>();
    public IEnumerable<IPvWayLogRow> LogRows => _rows;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, 
        string? topic, SeverityEnum severity, 
        string machineName, string memberName, 
        string filePath, int lineNumber, 
        string message, DateTime dateUtc)
    {
        var row = new LogRow(
            _lastId++,
            userId, companyId,
            topic, severity,
            machineName, memberName,
            filePath, lineNumber,
            message, dateUtc);
        _rows.Add(row);
        await Task.CompletedTask;
    }

    public void WriteLog(
        string? userId, string? companyId,
        string? topic, SeverityEnum severity,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        var row = new LogRow(
            _lastId++,
            userId, companyId,
            topic, severity,
            machineName, memberName,
            filePath, lineNumber,
            message, dateUtc);
        _rows.Add(row);
    }


    public bool HasLog(string term)
    {
        return _rows
            .OrderByDescending(x => x.Id)
            .FirstOrDefault(x => x.Message.Contains(term)) !=null;
    }

    public IPvWayLogRow? FindFirstMatchingRow(string term)
    {
        return _rows
            .OrderBy(x => x.Id)
            .FirstOrDefault(x => x.Message.Contains(term));
    }

    public IPvWayLogRow? FindLastMatchingRow(string term)
    {
        return _rows
            .OrderByDescending(x => x.Id)
            .FirstOrDefault(x => x.Message.Contains(term));
    }

}