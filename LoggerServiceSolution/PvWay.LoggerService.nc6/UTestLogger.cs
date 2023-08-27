using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

internal class UTestLogger : Logger, IPvWayUTestLoggerService
{
    private readonly IPvWayUTestLogWriter _lw;

    public IEnumerable<IPvWayLogRow> LogRows => _lw.LogRows;

    public UTestLogger(IPvWayUTestLogWriter lw) :
        base(lw)
    {
        _lw = lw;
    }


    public async Task WriteLogAsync(
        string? userId, string? companyId,
        string? topic, SeverityEnum severity,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        await _lw.WriteLogAsync(
            userId, companyId,
            topic, severity,
            machineName, memberName,
            filePath, lineNumber,
            message, dateUtc);
    }

    public void WriteLog(
        string? userId, string? companyId,
        string? topic, SeverityEnum severity,
        string machineName, string memberName,
        string filePath, int lineNumber,
        string message, DateTime dateUtc)
    {
        _lw.WriteLog(
            userId, companyId,
            topic, severity,
            machineName, memberName,
            filePath, lineNumber,
            message, dateUtc);
    }

    public bool HasLog(string term)
    {
        return _lw.HasLog(term);
    }

    public IPvWayLogRow? FindFirstMatchingRow(string term)
    {
        return _lw.FindFirstMatchingRow(term);
    }

    public IPvWayLogRow? FindLastMatchingRow(string term)
    {
        return _lw.FindLastMatchingRow(term);
    }

}