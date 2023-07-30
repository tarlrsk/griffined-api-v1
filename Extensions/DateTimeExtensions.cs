using System;
using System.Globalization;

namespace Extensions.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        // Extension method for convert DateTime to String.
        public static string ToFormattedString(this DateTime dateTime, string format = "dd-MMMM-yyyy")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Extension method to covert String to DateTime. This method returns null if parsing is failed.
        public static DateTime? ToDate(this string dateString, string format = "dd-MMMM-yyyy")
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}