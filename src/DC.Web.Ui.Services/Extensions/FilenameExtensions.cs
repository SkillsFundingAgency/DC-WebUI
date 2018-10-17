using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DC.Web.Ui.Services.Extensions
{
    public static class FileNameExtensions
    {
        public static string FileNameWithoutUkprn(this string fileName)
        {
            if (!fileName.Contains("/"))
            {
                return fileName.ToUpper();
            }

            return fileName.Split('/')[1].ToUpper();
        }

        public static string FileExtension(this string fileName)
        {
            if (!fileName.Contains("."))
            {
                return string.Empty;
            }

            return Path.GetExtension(fileName).Remove(0, 1);
        }
    }
}
