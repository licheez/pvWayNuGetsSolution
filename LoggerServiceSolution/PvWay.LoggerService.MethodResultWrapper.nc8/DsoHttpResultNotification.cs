// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using PvWay.LoggerService.Abstractions.nc8;

namespace PvWay.LoggerService.MethodResultWrapper.nc8;

public class DsoHttpResultNotification(
    SeverityEnu severity,
    string message)
{
    public string SeverityCode { get; } = EnumSeverity.GetCode(severity);
    public string Message { get; } = message;
}