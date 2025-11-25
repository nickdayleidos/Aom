using System;
using TimeZoneConverter; // Add TimeZoneConverter NuGet package

namespace MyApplication.Common.Time
{
    /// <summary>
    /// Provides conversion helpers between UTC, Eastern, and arbitrary site time zones.
    /// </summary>
    public static class Tz
    {
        /// <summary>
        /// Attempts to get a TimeZoneInfo for either IANA or Windows ID.
        /// </summary>
        public static TimeZoneInfo Resolve(string? tzId)
        {
            if (string.IsNullOrWhiteSpace(tzId))
                return Et.Zone; // fallback to Eastern

            try
            {
                // Works for either IANA or Windows
                return TZConvert.GetTimeZoneInfo(tzId);
            }
            catch
            {
                return Et.Zone; // fallback to Eastern
            }
        }

        /// <summary>
        /// Convert a UTC DateTime to the specified time zone (offset-aware).
        /// </summary>
        public static DateTimeOffset FromUtc(DateTime utc, string? tzId)
        {
            var zone = Resolve(tzId);
            var utcSpecified = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTime(new DateTimeOffset(utcSpecified), zone);
        }

        /// <summary>
        /// Convert a local time in the specified zone to UTC.
        /// </summary>
        public static DateTime ToUtc(DateTime local, string? tzId)
        {
            var zone = Resolve(tzId);
            var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, zone);
        }

        /// <summary>
        /// Convert a DateTime that’s currently stored in ET (legacy) to a site’s local time zone.
        /// </summary>
        public static DateTime FromEtToSite(DateTime etLocal, string? tzId)
        {
            var etOffset = new DateTimeOffset(etLocal, Et.Zone.GetUtcOffset(etLocal));
            var siteZone = Resolve(tzId);
            return TimeZoneInfo.ConvertTime(etOffset, siteZone).DateTime;
        }

        /// <summary>
        /// Convert a DateTime stored in a site’s local zone to ET.
        /// </summary>
        public static DateTime FromSiteToEt(DateTime siteLocal, string? tzId)
        {
            var siteZone = Resolve(tzId);
            var unspecified = DateTime.SpecifyKind(siteLocal, DateTimeKind.Unspecified);
            var offset = siteZone.GetUtcOffset(unspecified);

            var siteOffset = new DateTimeOffset(unspecified, offset);
            return TimeZoneInfo.ConvertTime(siteOffset, Et.Zone).DateTime;
        }



        /// <summary>
        /// Format helpers for consistency.
        /// </summary>
        public static string FormatSubject(DateTimeOffset dto)
            => dto.ToString(Et.SubjectFormat);

        public static string FormatBody(DateTimeOffset dto)
            => dto.ToString(Et.BodyFormat);
    }
}
