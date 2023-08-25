using Microsoft.Extensions.Logging;

namespace PvWay.LoggerService.Abstractions.nc6;

public enum SeverityEnum
{
    Ok,
    Debug,
    Info,
    Warning,
    Error,
    Fatal,
}

public static class EnumSeverity
{
    private const string Ok = "O";
    private const string Debug = "D";
    private const string Info = "I";
    private const string Warning = "W";
    private const string Error = "E";
    private const string Fatal = "F";

    public static string GetCode(SeverityEnum value)
    {
        return value switch
        {
            SeverityEnum.Ok => Ok,
            SeverityEnum.Debug => Debug,
            SeverityEnum.Info => Info,
            SeverityEnum.Warning => Warning,
            SeverityEnum.Error => Error,
            SeverityEnum.Fatal => Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static SeverityEnum GetValue(string code)
    {
        return code switch
        {
            null => SeverityEnum.Ok,
            Ok => SeverityEnum.Ok,
            Debug => SeverityEnum.Debug,
            Info => SeverityEnum.Info,
            Warning => SeverityEnum.Warning,
            Error => SeverityEnum.Error,
            Fatal => SeverityEnum.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };
    }

    public static string GetMsLogLevelAbbrev(LogLevel lgLevel)
    {
        return lgLevel switch
        {
            LogLevel.Trace => "trac:",
            // ReSharper disable once StringLiteralTypo
            LogLevel.Debug => "debg:",
            LogLevel.Information => "info:",
            LogLevel.Warning => "warn:",
            LogLevel.Error => "fail:",
            // ReSharper disable once StringLiteralTypo
            LogLevel.Critical => "crit:",
            LogLevel.None => "",
            _ => throw new ArgumentOutOfRangeException(nameof(lgLevel), lgLevel, null)
        };
    }

    public static LogLevel GetMsLogLevel(SeverityEnum severity)
    {
        return severity switch
        {
            SeverityEnum.Ok => LogLevel.None,
            SeverityEnum.Debug => LogLevel.Debug,
            SeverityEnum.Info => LogLevel.Information,
            SeverityEnum.Warning => LogLevel.Warning,
            SeverityEnum.Error => LogLevel.Error,
            SeverityEnum.Fatal => LogLevel.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }

    public static SeverityEnum GetSeverity(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => SeverityEnum.Debug,
            LogLevel.Debug => SeverityEnum.Debug,
            LogLevel.Information => SeverityEnum.Info,
            LogLevel.Warning => SeverityEnum.Warning,
            LogLevel.Error => SeverityEnum.Error,
            LogLevel.Critical => SeverityEnum.Fatal,
            LogLevel.None => SeverityEnum.Ok,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}