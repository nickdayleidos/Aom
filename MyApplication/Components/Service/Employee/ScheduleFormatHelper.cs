using MyApplication.Common.Time;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Employee;

/// <summary>
/// Shared helpers for formatting AcrSchedule data used by both
/// EmployeeListService and EmployeeScheduleService.
/// </summary>
internal static class ScheduleFormatHelper
{
    internal static string FormatSchedule(AcrSchedule? s, TimeDisplayMode mode)
    {
        if (s == null) return "-";

        var days = new[] {
            (d: "Mon", s: Convert(s.MondayStart, mode), e: Convert(s.MondayEnd, mode)),
            (d: "Tue", s: Convert(s.TuesdayStart, mode), e: Convert(s.TuesdayEnd, mode)),
            (d: "Wed", s: Convert(s.WednesdayStart, mode), e: Convert(s.WednesdayEnd, mode)),
            (d: "Thu", s: Convert(s.ThursdayStart, mode), e: Convert(s.ThursdayEnd, mode)),
            (d: "Fri", s: Convert(s.FridayStart, mode), e: Convert(s.FridayEnd, mode)),
            (d: "Sat", s: Convert(s.SaturdayStart, mode), e: Convert(s.SaturdayEnd, mode)),
            (d: "Sun", s: Convert(s.SundayStart, mode), e: Convert(s.SundayEnd, mode))
        };

        var active = days.Where(x => x.s.HasValue && x.e.HasValue).ToList();
        if (!active.Any()) return "OFF";

        var first = active.First();
        bool sameTime = active.All(x => x.s == first.s && x.e == first.e);

        if (sameTime)
        {
            string range = (active.Count == 5 && active[0].d == "Mon" && active[4].d == "Fri") ? "Mon-Fri" : $"{active.Count} Days";
            return $"{range} {first.s:HH:mm}-{first.e:HH:mm}";
        }
        return "Varies";
    }

    internal static List<DayScheduleDto> BuildDailySchedules(AcrSchedule? s, TimeDisplayMode mode)
    {
        var list = new List<DayScheduleDto>();
        if (s == null) return list;

        void Add(string d, TimeOnly? start, TimeOnly? end) =>
            list.Add(new DayScheduleDto { Day = d, Start = start, End = end });

        Add("Mon", Convert(s.MondayStart, mode), Convert(s.MondayEnd, mode));
        Add("Tue", Convert(s.TuesdayStart, mode), Convert(s.TuesdayEnd, mode));
        Add("Wed", Convert(s.WednesdayStart, mode), Convert(s.WednesdayEnd, mode));
        Add("Thu", Convert(s.ThursdayStart, mode), Convert(s.ThursdayEnd, mode));
        Add("Fri", Convert(s.FridayStart, mode), Convert(s.FridayEnd, mode));
        Add("Sat", Convert(s.SaturdayStart, mode), Convert(s.SaturdayEnd, mode));
        Add("Sun", Convert(s.SundayStart, mode), Convert(s.SundayEnd, mode));

        return list;
    }

    internal static TimeOnly? Convert(TimeOnly? time, TimeDisplayMode mode)
    {
        if (!time.HasValue) return null;
        if (mode == TimeDisplayMode.Eastern) return time;

        try
        {
            var et = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var targetZoneId = mode == TimeDisplayMode.Mountain ? "Mountain Standard Time" : "Eastern Standard Time";
            var target = TimeZoneInfo.FindSystemTimeZoneById(targetZoneId);
            var today = DateTime.Today;
            var dtEt = new DateTime(today.Year, today.Month, today.Day, time.Value.Hour, time.Value.Minute, 0);
            var dtTarget = TimeZoneInfo.ConvertTime(dtEt, et, target);
            return TimeOnly.FromDateTime(dtTarget);
        }
        catch { return time; }
    }
}
