using System.Text.Json.Serialization;

namespace PvWay.LoggerService.Abstractions.nc6;

public sealed class PvWayLoggerServiceConfig
{
    public const string Section = nameof(PvWayLoggerServiceConfig);

    // ReSharper disable once MemberCanBePrivate.Global
    public string MinLogLevel { get; set; } = "Trace";

    [JsonIgnore]
    public SeverityEnu MinLevel => MinLogLevel.ToLower() switch
    {
        "trace" or "t" or "verbose" or "v" => SeverityEnu.Trace,
        "debug" or "d" => SeverityEnu.Debug,
        "info" or "information" or "i" => SeverityEnu.Info,
        "warning" or "w" => SeverityEnu.Warning,
        "error" or "e" => SeverityEnu.Error,
        "fatal" or "f" or "critic" or "critical" or "c" => SeverityEnu.Fatal,
        _ => SeverityEnu.Trace
    };
}