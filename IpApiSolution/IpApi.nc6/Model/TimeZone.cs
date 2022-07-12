using pvWay.IpApi.nc6.interfaces;

namespace pvWay.IpApi.nc6.Model;

internal class TimeZone : ITimeZone
{
    public string? Id { get; }
    public DateTime? CurrentTime { get; }
    public int? GmtOffset { get; }
    public string? Code { get; }
    public bool? IsDayLightSaving { get; }

    public TimeZone(dynamic rd)
    {
        Id = rd.id;
        CurrentTime = rd.current_time == null
            ? null
            : new DateTime(rd.current_time);
        GmtOffset = rd.gmt_offset;
        Code = rd.code;
        IsDayLightSaving = rd.is_daylight_saving;
    }
}