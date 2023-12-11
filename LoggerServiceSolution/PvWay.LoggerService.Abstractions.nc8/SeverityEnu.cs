using Microsoft.Extensions.Logging;

namespace PvWay.LoggerService.Abstractions.nc8;

public enum SeverityEnu
{
    Ok,
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Fatal,
}

public static class EnumSeverity
{
    private const string Ok = "O";
    private const string Trace = "T";
    private const string Debug = "D";
    private const string Info = "I";
    private const string Warning = "W";
    private const string Error = "E";
    private const string Fatal = "F";

    public static string GetCode(SeverityEnu value)
    {
        return value switch
        {
            SeverityEnu.Ok => Ok,
            SeverityEnu.Trace => Trace,
            SeverityEnu.Debug => Debug,
            SeverityEnu.Info => Info,
            SeverityEnu.Warning => Warning,
            SeverityEnu.Error => Error,
            SeverityEnu.Fatal => Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static SeverityEnu GetValue(string code)
    {
        return code switch
        {
            null => SeverityEnu.Ok,
            Ok => SeverityEnu.Ok,
            Trace => SeverityEnu.Trace,
            Debug => SeverityEnu.Debug,
            Info => SeverityEnu.Info,
            Warning => SeverityEnu.Warning,
            Error => SeverityEnu.Error,
            Fatal => SeverityEnu.Fatal,
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

    public static LogLevel GetMsLogLevel(SeverityEnu severity)
    {
        return severity switch
        {
            SeverityEnu.Ok => LogLevel.None,
            SeverityEnu.Trace => LogLevel.Trace,
            SeverityEnu.Debug => LogLevel.Debug,
            SeverityEnu.Info => LogLevel.Information,
            SeverityEnu.Warning => LogLevel.Warning,
            SeverityEnu.Error => LogLevel.Error,
            SeverityEnu.Fatal => LogLevel.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }

    public static SeverityEnu GetSeverity(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => SeverityEnu.Trace,
            LogLevel.Debug => SeverityEnu.Debug,
            LogLevel.Information => SeverityEnu.Info,
            LogLevel.Warning => SeverityEnu.Warning,
            LogLevel.Error => SeverityEnu.Error,
            LogLevel.Critical => SeverityEnu.Fatal,
            LogLevel.None => SeverityEnu.Ok,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}