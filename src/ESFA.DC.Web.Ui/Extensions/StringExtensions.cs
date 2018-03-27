using System;
using System.Linq;

namespace DC.Web.Ui.Extensions
{
    public static class StringExtensions
    {
        private static Random random = new Random();

        public static string AppendRandomString(this string value, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{value}-{randomString}";
        }
    }
}