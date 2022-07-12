namespace pvWay.IpApi.nc6.interfaces;

public interface ITimeZone
{
    string? Id { get; }
    DateTime? CurrentTime { get; }
    int? GmtOffset { get; }
    string? Code { get; }
    bool? IsDayLightSaving { get; }
}