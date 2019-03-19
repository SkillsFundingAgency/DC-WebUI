using System.Globalization;

namespace DC.Web.Ui.Services.Extensions
{
    public static class AcadmicPeriodYearExtensions
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

        public static string ToAcademicYearName(this int academicYear)
        {
            return $"20{academicYear.ToString().Substring(0, 2)} to 20{academicYear.ToString().Substring(2)}";
        }
    }
}
