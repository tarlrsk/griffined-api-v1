using System;
using System.Globalization;

namespace griffined_api.Extensions.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        // Extension method to convert DateTime to String (format: dd-MMMM-yyyy).
        public static string ToDateString(this DateTime dateTime, string format = "dd MMM yyyy")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to convert TimeOnly to String (format: HH:mm)
        public static string ToTimeSpanString(this TimeSpan time, string format = @"hh\:mm")
        {
            return time.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to convert DateTime to String (format: dd-MMMM-yyyy HH:mm:ss).
        public static string ToDateTimeString(this DateTime dateTime, string format = "dd MMM yyyy HH:mm:ss")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to convert DateTime to String (format: ddd dd-MMMM-yyyy)
        public static string ToDateWithDayString(this DateTime dateTime, string format = "dddd, dd MMM yyyy")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to covert String to DateTime. This method throws an argument exception when the date format or date string is invalid.
        public static DateTime ToDateTime(this string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Invalid date format or date string.", nameof(dateString));
            }
        }

        // Extension method to convert String to TimeSpan. This method throws an argument exception when the time format or time string is invalid.
        public static TimeSpan ToTimeSpan(this string timeString)
        {
            if (TimeSpan.TryParse(timeString, out TimeSpan result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Invalid time format or time string.", nameof(timeString));
            }
        }
    }
}