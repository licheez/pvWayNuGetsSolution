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
}