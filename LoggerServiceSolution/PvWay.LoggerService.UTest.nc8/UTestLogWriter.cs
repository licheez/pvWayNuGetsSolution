using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.nc8;

namespace PvWay.LoggerService.UTest.nc8;

internal sealed class UTestLogWriter: IUTestLogWriter
{
    private int _lastId;
    private readonly IList<ILoggerServiceRow> _rows = new List<ILoggerServiceRow>();
    public IEnumerable<ILoggerServiceRow> LogRows => _rows;

    public void Dispose()
    {
        // nop
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    public async Task WriteLogAsync(
        string? userId, string? companyId, 
        string? topic, SeverityEnu severity, 
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
        string? topic, SeverityEnu severity,
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

    public ILoggerServiceRow? FindFirstMatchingRow(string term)
    {
        return _rows
            .OrderBy(x => x.Id)
            .FirstOrDefault(x => x.Message.Contains(term));
    }

    public ILoggerServiceRow? FindLastMatchingRow(string term)
    {
        return _rows
            .OrderByDescending(x => x.Id)
            .FirstOrDefault(x => x.Message.Contains(term));
    }

}