using MudBlazor;
using System;
using System.Text.RegularExpressions;

namespace MyApplication.Components.Common.Time
{
    public static class TimeInputHelpers
    {
        // Custom converter for MudTimePicker to allow inputs like "859", "0859", "8:59"
        public static Converter<TimeSpan?> FlexibleTimeConverter = new Converter<TimeSpan?>
        {
            SetFunc = value => value.HasValue ? $"{value.Value.Hours:D2}:{value.Value.Minutes:D2}" : string.Empty,
            GetFunc = text => ParseTime(text)
        };

        private static TimeSpan? ParseTime(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // Remove spaces and trim
            var clean = input.Trim().Replace(" ", "");

            // 1. Handle Standard Colon Format (e.g., "8:59", "08:59")
            if (clean.Contains(":"))
            {
                if (TimeSpan.TryParse(clean, out var ts)) return ts;
                if (DateTime.TryParse(clean, out var dt)) return dt.TimeOfDay;
            }
            // 2. Handle Continuous Digits (e.g., "859", "0859", "1300")
            else if (Regex.IsMatch(clean, @"^\d{3,4}$"))
            {
                // Pad to ensure 4 digits: "859" -> "0859"
                var padded = clean.PadLeft(4, '0');

                if (int.TryParse(padded.Substring(0, 2), out int h) &&
                    int.TryParse(padded.Substring(2, 2), out int m))
                {
                    // Basic validation
                    if (h >= 0 && h < 24 && m >= 0 && m < 60)
                    {
                        return new TimeSpan(h, m, 0);
                    }
                }
            }

            // Return null if parsing fails (MudBlazor will usually clear or show error depending on config)
            return null;
        }
    }
}