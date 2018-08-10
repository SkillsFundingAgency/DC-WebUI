using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Web.Ui.Extensions
{
    public static class PeriodExnensions
    {
        public static string ToPeriodName(this int periodNumber)
        {
            return $"R{periodNumber.ToString("00", NumberFormatInfo.InvariantInfo)}";
        }

        public static int ToPeriodNumber(this string periodName)
        {
            int.TryParse(periodName.Replace("R", string.Empty), out var result);
            return result;
        }
    }
}
