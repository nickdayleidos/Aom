using MyApplication.Common.Time;
using SDateTime = global::System.DateTime;
using SDateTimeOffset = global::System.DateTimeOffset;
using STz = global::System.TimeZoneInfo;

namespace MyApplication.Common.Time // Ensure namespace matches folder structure
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
        TimeOnly ToLocal(TimeOnly et, string? employeeSiteTzId);

        // --- NEW METHOD ---
        // Converts a local time (e.g. 8:00 AM Mountain) to Eastern (10:00 AM Eastern)
        TimeOnly ToEastern(TimeOnly local, string? employeeSiteTzId);
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

        public TimeOnly ToLocal(TimeOnly et, string? employeeSiteTzId)
        {
            if (string.IsNullOrWhiteSpace(employeeSiteTzId)) return et;

            try
            {
                var localZone = STz.FindSystemTimeZoneById(employeeSiteTzId);

                // Use Today's date to handle DST correctly
                var now = SDateTime.Now;
                // Create Date in Eastern Time
                var etDt = new SDateTime(now.Year, now.Month, now.Day, et.Hour, et.Minute, 0, System.DateTimeKind.Unspecified);

                // Convert ET -> Local
                var localDt = STz.ConvertTime(etDt, Et.Zone, localZone);

                return TimeOnly.FromDateTime(localDt);
            }
            catch
            {
                return et; // Fallback
            }
        }

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
            // Treat the input as Unspecified or Eastern, then get offset
            var etOffset = new SDateTimeOffset(etLocal, Et.Zone.GetUtcOffset(etLocal));
            return STz.ConvertTime(etOffset, tz);
        }

        public string FormatEt(SDateTime etLocal, string? employeeSiteTzId, string format = Et.BodyFormat)
            => FromEt(etLocal, employeeSiteTzId).ToString(format);

        // --- IMPLEMENTATION OF NEW METHOD ---
        public TimeOnly ToEastern(TimeOnly local, string? employeeSiteTzId)
        {
            // If no site is provided, we can't convert (or assume it's already ET)
            if (string.IsNullOrEmpty(employeeSiteTzId)) return local;

            try
            {
                // 1. Get the Source Time Zone (e.g., "Mountain Standard Time")
                var sourceZone = STz.FindSystemTimeZoneById(employeeSiteTzId);

                // 2. Create a DateTime for "Today" at that time in the Source Zone
                // We use DateTime.Now to correctly handle Daylight Saving Time for the current date.
                var now = SDateTime.Now;
                var sourceDt = new SDateTime(now.Year, now.Month, now.Day, local.Hour, local.Minute, 0, System.DateTimeKind.Unspecified);

                // 3. Convert from Source Zone -> Eastern Zone
                var easternDt = STz.ConvertTime(sourceDt, sourceZone, Et.Zone);

                // 4. Return the time portion
                return TimeOnly.FromDateTime(easternDt);
            }
            catch
            {
                // If the TimeZone ID is invalid (e.g. "NULL" string or unknown), return original time
                return local;
            }
        }
    }
}