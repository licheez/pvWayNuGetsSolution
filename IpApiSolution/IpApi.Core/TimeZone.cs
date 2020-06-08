using System;

namespace pvWay.IpApi.Core
{
    internal class TimeZone : ITimeZone
    {
        public string Id { get; }
        public DateTime CurrentTime { get; }
        public int GmtOffset { get; }
        public string Code { get; }
        public bool IsDayLightSaving { get; }

        public TimeZone(dynamic rd)
        {
            Id = rd.id;
            CurrentTime = new DateTime(rd.current_time);
            GmtOffset = rd.gmt_offset;
            Code = rd.code;
            IsDayLightSaving = rd.is_daylight_saving;
        }
    }
}