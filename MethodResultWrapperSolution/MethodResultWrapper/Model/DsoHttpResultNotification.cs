// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using pvWay.MethodResultWrapper.Enums;
using pvWay.MethodResultWrapper.Interfaces;

namespace pvWay.MethodResultWrapper.Model
{
    public class DsoHttpResultNotification : IDsoHttpResultNotification
    {
        public string SeverityCode { get; }
        public string Message { get; }

        public DsoHttpResultNotification(
            SeverityEnum severity,
            string message)
        {
            SeverityCode = EnumSeverity.GetCode(severity);
            Message = message;
        }

    }
}
