namespace PvWay.LoggerService.Abstractions.nc8;

public interface IUTestLogWriter: ILogWriter
{
    public IEnumerable<ILoggerServiceRow> LogRows { get; }
    /// <summary>
    /// Checks if the logRows have a row where
    /// the row.message contains the provided term
    /// </summary>
    /// <param name="term"></param>
    /// <returns></returns>
    public bool HasLog(string term);
    public ILoggerServiceRow? FindFirstMatchingRow(string term);
    public ILoggerServiceRow? FindLastMatchingRow(string term);
}
