using System;
using System.Globalization;

namespace Extensions.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        // Extension method to convert DateTime to String (format: dd-MMMM-yyyy).
        public static string ToDateString(this DateTime dateTime, string format = "dd-MMMM-yyyy")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to convert TimeOnly to String (format: HH:mm:ss)
        public static string ToTimeString(this TimeOnly time, string format = "HH:mm")
        {
            return time.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to convert DateTime to String (format: dd-MMMM-yyyy HH:mm:ss).
        public static string ToDateTimeString(this DateTime dateTime, string format = "dd-MMMM-yy HH:mm:ss")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to covert String to DateTime. This method throws an argument exception when the date format or date string is invalid.
        public static DateTime ToDateTime(this string dateString)
        {
            DateTime result;

            if (DateTime.TryParse(dateString, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Invalid date format or date string.", nameof(dateString));
            }
        }

        // Extension method to convert String to TimeOnly. This method throws an argument exception when the time format or time string is invalid.
        public static TimeOnly ToTime(this string timeString)
        {
            TimeOnly result;

            if (TimeOnly.TryParse(timeString, out result))
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