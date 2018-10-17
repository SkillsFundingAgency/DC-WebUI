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
    }
}
