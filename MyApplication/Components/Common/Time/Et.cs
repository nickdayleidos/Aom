using System;

namespace MyApplication.Common.Time
{
    public static class Et
    {
        public const string WindowsId = "Eastern Standard Time";
        public const string IanaId = "America/New_York";

        // cache the zone once per app lifetime
        private static readonly TimeZoneInfo _zone = GetEtZone();
        public static TimeZoneInfo Zone => _zone;

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _zone);
        public static DateTimeOffset NowOffset
            => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _zone);

        public static DateOnly Today => DateOnly.FromDateTime(Now);

        public static DateTime FromUtc(DateTime utc)    // UTC -> ET (naive local)
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), _zone);

        public static DateTime ToUtc(DateTime etLocal)  // ET (naive local) -> UTC
            => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(etLocal, DateTimeKind.Unspecified), _zone);

        public static DateTimeOffset ToEtOffset(DateTime utc) // UTC -> ET (offset-aware)
        {
            var etLocal = FromUtc(utc);
            return new DateTimeOffset(etLocal, _zone.GetUtcOffset(etLocal));
        }

        // common formats (keep them consistent across emails)
        public const string SubjectFormat = "yyyy-MM-dd HH:mm";       // e.g., 2025-09-04 12:35
        public const string BodyFormat = "MM/dd/yyyy h:mm tt";     // e.g., 09/04/2025 12:35 PM

        private static TimeZoneInfo GetEtZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(WindowsId); }
            catch
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(IanaId); }
                catch { return TimeZoneInfo.Utc; } // last-resort fallback
            }
        }
    }
}
