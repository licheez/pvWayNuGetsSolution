﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using PvWay.LoggerService.Abstractions.nc6;

namespace PvWay.LoggerService.MethodResultWrapper.nc6;

public class DsoHttpResultNotification
{
    public DsoHttpResultNotification(SeverityEnu severity,
        string message)
    {
        SeverityCode = EnumSeverity.GetCode(severity);
        Message = message;
    }

    public string SeverityCode { get; }
    public string Message { get; }
}