// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MethodResultWrapper.Core
{
    public class DsoHttpResultNotification
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
