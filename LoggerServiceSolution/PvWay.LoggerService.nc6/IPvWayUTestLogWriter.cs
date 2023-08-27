using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.nc6;

public interface IPvWayUTestLogWriter: ILogWriter
{
    public IEnumerable<IPvWayLogRow> LogRows { get; }
    /// <summary>
    /// Checks if the logRows have a row where
    /// the row.message contains the provided term
    /// </summary>
    /// <param name="term"></param>
    /// <returns></returns>
    public bool HasLog(string term);
    public IPvWayLogRow? FindFirstMatchingRow(string term);
    public IPvWayLogRow? FindLastMatchingRow(string term);
}

public interface IPvWayUTestLoggerService : ILoggerService, IPvWayUTestLogWriter
{
}