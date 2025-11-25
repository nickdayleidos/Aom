using SDateTime = global::System.DateTime;
using SDateTimeOffset = global::System.DateTimeOffset;
using STz = global::System.TimeZoneInfo;

namespace MyApplication.Common.Time
{
    public static class Mt
    {
        public const string WindowsId = "Mountain Standard Time";
        public const string IanaId = "America/Denver";

        private static readonly STz _zone = GetMtZone();
        public static STz Zone => _zone;

        public static SDateTime Now => STz.ConvertTimeFromUtc(SDateTime.UtcNow, _zone);
        public static SDateTimeOffset NowOffset => STz.ConvertTime(SDateTimeOffset.UtcNow, _zone);
        public static DateOnly Today => DateOnly.FromDateTime(Now);

        public static SDateTime FromUtc(SDateTime utc)
            => STz.ConvertTimeFromUtc(SDateTime.SpecifyKind(utc, System.DateTimeKind.Utc), _zone);

        public static SDateTime ToUtc(SDateTime mtLocal)
            => STz.ConvertTimeToUtc(SDateTime.SpecifyKind(mtLocal, System.DateTimeKind.Unspecified), _zone);

        public static SDateTimeOffset ToMtOffset(SDateTime utc)
        {
            var mtLocal = FromUtc(utc);
            return new SDateTimeOffset(mtLocal, _zone.GetUtcOffset(mtLocal));
        }

        private static STz GetMtZone()
        {
            try { return STz.FindSystemTimeZoneById(WindowsId); }
            catch
            {
                try { return STz.FindSystemTimeZoneById(IanaId); }
                catch { return STz.Utc; }
            }
        }
    }
}
