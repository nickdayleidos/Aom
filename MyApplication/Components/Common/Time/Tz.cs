using System;

namespace MyApplication.Common.Time
{
    public static class Tz
    {
        public static TimeZoneInfo Resolve(string? tzId)
        {
            if (string.IsNullOrWhiteSpace(tzId)) return Et.Zone;
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(tzId);
            }
            catch
            {
                // Fallback if ID is invalid on this OS
                return Et.Zone;
            }
        }

        public static DateTimeOffset FromUtc(DateTime utc, string? tzId)
        {
            var zone = Resolve(tzId);
            var utcSpecified = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTime(new DateTimeOffset(utcSpecified), zone);
        }

        public static DateTime ToUtc(DateTime local, string? tzId)
        {
            var zone = Resolve(tzId);
            var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, zone);
        }

        public static DateTime FromEtToSite(DateTime etLocal, string? tzId)
        {
            var etOffset = new DateTimeOffset(etLocal, Et.Zone.GetUtcOffset(etLocal));
            var siteZone = Resolve(tzId);
            return TimeZoneInfo.ConvertTime(etOffset, siteZone).DateTime;
        }

        public static DateTime FromSiteToEt(DateTime siteLocal, string? tzId)
        {
            var siteZone = Resolve(tzId);
            var unspecified = DateTime.SpecifyKind(siteLocal, DateTimeKind.Unspecified);
            var offset = siteZone.GetUtcOffset(unspecified);

            var siteOffset = new DateTimeOffset(unspecified, offset);
            return TimeZoneInfo.ConvertTime(siteOffset, Et.Zone).DateTime;
        }

        public static string FormatSubject(DateTimeOffset dto)
            => dto.ToString(Et.SubjectFormat);

        public static string FormatBody(DateTimeOffset dto)
            => dto.ToString(Et.BodyFormat);
    }
}