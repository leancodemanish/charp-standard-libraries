using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LeanCode.Standard.TypeExt
{
    public static class StringExt
    {
        public static DateTime ParseAsDateTime(this string mayBeDateTime)
        {
            var convertedDate = TryParseAsDateTime(mayBeDateTime);
            if (convertedDate == null) return DateTime.MinValue;
            return convertedDate.Value;
        }

        public static DateTime ParseAsDateTimeExact(this string mayBeDateTime, string format)
        {
            return DateTime.ParseExact(mayBeDateTime, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);
        }

        public static DateTime? TryParseAsDateTime(this string mayBeDateTime)
        {
            switch (mayBeDateTime.ToLower())
            {
                case "now":
                    return DateTime.Now;

                case "utcnow":
                    return DateTime.UtcNow;

                case "today":
                    return DateTime.Today;

                case "yesterday":
                case "yday":
                    return DateTime.Today.AddDays(-1);

                case "tomorrow":
                    return DateTime.Today.AddDays(1);
            }

            if (DateTime.TryParse(mayBeDateTime, out var parsedDate))
            {
                return parsedDate;
            }

            if (DateTime.TryParseExact(mayBeDateTime, "yyyyMMdd HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out parsedDate))
            {
                return parsedDate;
            }

            if (int.TryParse(mayBeDateTime, out var v))
            {
                return new DateTime(v / 10000, (v / 100) % 100, v % 100);
            }

            return null;
        }
    }
}
