using SDateTime = global::System.DateTime;
using SDateTimeOffset = global::System.DateTimeOffset;
using STz = global::System.TimeZoneInfo;

namespace MyApplication.Common.Time
{
    public enum SiteZone { ET, MT }

    public static class SiteTime
    {
        public static SiteZone Parse(string? siteTz)
        {
            if (string.IsNullOrWhiteSpace(siteTz)) return SiteZone.ET;
            siteTz = siteTz.Trim().ToUpperInvariant();
            return siteTz switch
            {
                "MT" or "MOUNTAIN" or "MOUNTAIN STANDARD TIME" or "AMERICA/DENVER" => SiteZone.MT,
                _ => SiteZone.ET
            };
        }

        public static STz Resolve(SiteZone z) => z == SiteZone.MT ? Mt.Zone : Et.Zone;

        public static SDateTimeOffset FromUtc(SDateTime utc, SiteZone z)
        {
            var dtoUtc = new SDateTimeOffset(SDateTime.SpecifyKind(utc, System.DateTimeKind.Utc));
            return STz.ConvertTime(dtoUtc, Resolve(z));
        }
        public static DateTime FromEtToSite(DateTime etTime, SiteZone zone)
        {
            if (zone == SiteZone.ET) return etTime;
            return Tz.FromEtToSite(etTime, GetTzId(zone));
        }
        private static string GetTzId(SiteZone zone)
        {
            return zone == SiteZone.MT ? "Mountain Standard Time" : "Eastern Standard Time";
        }

        public static SDateTime ToUtc(SDateTime siteLocal, SiteZone z)
        {
            var unspecified = SDateTime.SpecifyKind(siteLocal, System.DateTimeKind.Unspecified);
            return STz.ConvertTimeToUtc(unspecified, Resolve(z));
        }

        // DB stored in ET (naive) -> site local
        public static SDateTime FromEtStoredToSite(SDateTime etLocal, SiteZone z)
        {
            var etOffset = new SDateTimeOffset(etLocal, Et.Zone.GetUtcOffset(etLocal));
            return STz.ConvertTime(etOffset, Resolve(z)).DateTime;
        }

        // site local -> ET (for writing ET DB)
        public static SDateTime FromSiteToEt(SDateTime siteLocal, SiteZone z)
        {
            // Treat the picker value as a naive "local" time in the chosen site zone
            var zone = Resolve(z);
            var unspecified = SDateTime.SpecifyKind(siteLocal, DateTimeKind.Unspecified);
            var offset = zone.GetUtcOffset(unspecified);

            var siteOffset = new SDateTimeOffset(unspecified, offset);
            return STz.ConvertTime(siteOffset, Et.Zone).DateTime;
        }

    }

    public static partial class Et
    {
        public static SDateTime ToMt(SDateTime etLocal)
            => STz.ConvertTime(new SDateTimeOffset(etLocal, Zone.GetUtcOffset(etLocal)), Mt.Zone).DateTime;

        public static SDateTime FromMt(SDateTime mtLocal)
            => STz.ConvertTime(new SDateTimeOffset(mtLocal, Mt.Zone.GetUtcOffset(mtLocal)), Zone).DateTime;
    }
}
