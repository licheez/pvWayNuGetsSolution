using System.Collections.Generic;
using pvWay.MethodResult.enums;

namespace pvWay.MethodResult.interfaces
{
    public interface IMethodResult
    {
        bool Failure { get; }
        bool Success { get; }
        SeverityEnum Severity { get; }
        string ErrorMessage { get; }
        IEnumerable<IMethodResultNotification> Notifications { get; }
        void Throw();
    }

    public interface IMethodResult<out T> : IMethodResult
    {
        T Data { get; }
    }

    public interface IMethodResultNotification
    {
        SeverityEnum Severity { get; }
        string Message { get; }
    }
}