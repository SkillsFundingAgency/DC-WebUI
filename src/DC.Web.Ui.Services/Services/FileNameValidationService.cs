using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class FileNameValidationService : IFileNameValidationService
    {
        private readonly Regex _fileNameRegex = new Regex("^(ILR)-([1-9][0-9]{7})-(1819)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-(([1-9][0-9])|(0[1-9])).((XML)|(ZIP)|(xml)|(zip))$", RegexOptions.Compiled);

        public FileNameValidationResult ValidateFile(string fileName, long ukprn)
        {
            if (!_fileNameRegex.IsMatch(fileName))
            {
                return FileNameValidationResult.InvalidFileNameFormat;
            }

            var matches = _fileNameRegex.Match(fileName);
            var fileUkprn = long.Parse(matches.Groups[2].Value);

            if (fileUkprn != ukprn)
            {
                return FileNameValidationResult.UkprnDifferentToFileName;
            }

            return FileNameValidationResult.Valid;
        }
    }
}
