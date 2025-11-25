using SDateTime = global::System.DateTime;
using SDateTimeOffset = global::System.DateTimeOffset;
using STz = global::System.TimeZoneInfo;

namespace MyApplication.Common.Time
{
    public enum TimeDisplayMode { EmployeeLocal, Eastern, Mountain }

    public interface ITimeDisplayService
    {
        TimeDisplayMode Mode { get; set; }
        STz ResolveZone(string? employeeSiteTzId);
        SDateTimeOffset FromUtc(SDateTime utc, string? employeeSiteTzId);
        string FormatUtc(SDateTime utc, string? employeeSiteTzId, string format = Et.BodyFormat);
        SDateTimeOffset FromEt(SDateTime etLocal, string? employeeSiteTzId);
        string FormatEt(SDateTime etLocal, string? employeeSiteTzId, string format = Et.BodyFormat);
    }

    public sealed class TimeDisplayService : ITimeDisplayService
    {
        public TimeDisplayMode Mode { get; set; } = TimeDisplayMode.EmployeeLocal;

        public STz ResolveZone(string? employeeSiteTzId)
        {
            return Mode switch
            {
                TimeDisplayMode.Eastern => Et.Zone,
                TimeDisplayMode.Mountain => Mt.Zone,
                _ => SiteTime.Parse(employeeSiteTzId) == SiteZone.MT ? Mt.Zone : Et.Zone
            };
        }

        // TimeZoneInfo.ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)

        public SDateTimeOffset FromUtc(SDateTime utc, string? employeeSiteTzId)
        {
            var tz = ResolveZone(employeeSiteTzId);
            var dtoUtc = new SDateTimeOffset(SDateTime.SpecifyKind(utc, System.DateTimeKind.Utc));
           
              return STz.ConvertTime(dateTimeOffset: dtoUtc, destinationTimeZone: tz);
        
}


        public string FormatUtc(SDateTime utc, string? employeeSiteTzId, string format = Et.BodyFormat)
            => FromUtc(utc, employeeSiteTzId).ToString(format);

        public SDateTimeOffset FromEt(SDateTime etLocal, string? employeeSiteTzId)
        {
            var tz = ResolveZone(employeeSiteTzId);
            var etOffset = new SDateTimeOffset(etLocal, Et.Zone.GetUtcOffset(etLocal));
            return STz.ConvertTime(etOffset, tz);
        }

        public string FormatEt(SDateTime etLocal, string? employeeSiteTzId, string format = Et.BodyFormat)
            => FromEt(etLocal, employeeSiteTzId).ToString(format);
    }
}
