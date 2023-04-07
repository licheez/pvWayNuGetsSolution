namespace MsSqlLogWriter.nc6
{
    public interface IMsSqlLogWriter: IDisposable
    {
        void WriteLog(
            string userId, string companyId, string topic,
            string severityCode, string machineName,
            string memberName, string filePath, int lineNumber,
            string message, DateTime dateUtc);

        Task WriteLogAsync(
            string userId, string companyId, string topic,
            string severityCode, string machineName,
            string memberName, string filePath, int lineNumber,
            string message, DateTime dateUtc);
    }
}