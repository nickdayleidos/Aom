using SDateTime = global::System.DateTime;
using SDateTimeOffset = global::System.DateTimeOffset;
using STz = global::System.TimeZoneInfo;

namespace MyApplication.Common.Time
{
    public static partial class Et
    {
        public const string WindowsId = "Eastern Standard Time";
        public const string IanaId = "America/New_York";

        private static readonly STz _zone = GetEtZone();
        public static STz Zone => _zone;

        public static SDateTime Now => STz.ConvertTimeFromUtc(SDateTime.UtcNow, _zone);
        public static SDateTimeOffset NowOffset => STz.ConvertTime(SDateTimeOffset.UtcNow, _zone);
        public static DateOnly Today => DateOnly.FromDateTime(Now);

        public static SDateTime FromUtc(SDateTime utc)
            => STz.ConvertTimeFromUtc(SDateTime.SpecifyKind(utc, System.DateTimeKind.Utc), _zone);

        public static SDateTime ToUtc(SDateTime etLocal)
            => STz.ConvertTimeToUtc(SDateTime.SpecifyKind(etLocal, System.DateTimeKind.Unspecified), _zone);

        public static SDateTimeOffset ToEtOffset(SDateTime utc)
        {
            var etLocal = FromUtc(utc);
            return new SDateTimeOffset(etLocal, _zone.GetUtcOffset(etLocal));
        }

        public const string SubjectFormat = "yyyy-MM-dd HH:mm";
        public const string BodyFormat = "MM/dd/yyyy h:mm tt";

        private static STz GetEtZone()
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
