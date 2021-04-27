using System;
using System.Threading.Tasks;

namespace pvWay.MethodResultWrapper.Interfaces
{
    public interface ILogWriter : IDisposable
    {
        Task WriteLogAsync(
            string userId, string companyId, string topic,
            string severityCode,
            string machineName, string memberName,
            string filePath, int lineNumber,
            string message, DateTime dateUtc);

        void WriteLog(
            string userId, string companyId, string topic,
            string severityCode,
            string machineName, string memberName,
            string filePath, int lineNumber,
            string message, DateTime dateUtc);
    }
}