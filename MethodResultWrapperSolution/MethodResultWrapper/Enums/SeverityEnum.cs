using System;

namespace pvWay.MethodResultWrapper.Enums
{
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
            switch (value)
            {
                case SeverityEnum.Ok:
                    return Ok;
                case SeverityEnum.Debug:
                    return Debug;
                case SeverityEnum.Info:
                    return Info;
                case SeverityEnum.Warning:
                    return Warning;
                case SeverityEnum.Error:
                    return Error;
                case SeverityEnum.Fatal:
                    return Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public static SeverityEnum GetValue(string code)
        {
            switch (code)
            {
                case null:
                case Ok:
                    return SeverityEnum.Ok;
                case Debug:
                    return SeverityEnum.Debug;
                case Info:
                    return SeverityEnum.Info;
                case Warning:
                    return SeverityEnum.Warning;
                case Error:
                    return SeverityEnum.Error;
                case Fatal:
                    return SeverityEnum.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }

}
