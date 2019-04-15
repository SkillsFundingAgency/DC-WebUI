using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Web.Ui.Services.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateDisplayFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd MMMM yyyy");
        }

        public static string ToDateTimeDisplayFormat(this DateTime dateTime)
        {
            return $"{dateTime.ToString("dd MMMM yyyy")} at {dateTime.ToString("hh: mmtt").ToLower()}";
        }

        public static string ToDateWithDayDisplayFormat(this DateTime dateTime)
        {
            return $"{dateTime.ToString("dddd dd MMMM")}";
        }
    }
}
