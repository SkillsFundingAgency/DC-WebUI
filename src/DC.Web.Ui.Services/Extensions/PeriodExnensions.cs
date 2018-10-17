using System.Globalization;

namespace DC.Web.Ui.Services.Extensions
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
