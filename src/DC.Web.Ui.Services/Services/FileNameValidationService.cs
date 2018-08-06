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

        public FileNameValidationResult ValidateFileName(string fileName, long? fileSize, long ukprn)
        {
            if (string.IsNullOrEmpty(fileName) || fileSize == null || fileSize.Value == 0)
            {
                return FileNameValidationResult.EmptyFile;
            }

            if (!IsValidExtension(fileName))
            {
                return FileNameValidationResult.InvalidFileExtension;
            }

            if (!IsValidRegex(fileName))
            {
                return FileNameValidationResult.InvalidFileNameFormat;
            }

            if (!IsValidUkprn(fileName, ukprn))
            {
                return FileNameValidationResult.UkprnDifferentToFileName;
            }

            return FileNameValidationResult.Valid;
        }

        public bool IsValidExtension(string fileName)
        {
            return fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase) ||
                   fileName.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsValidRegex(string fileName)
        {
            return _fileNameRegex.IsMatch(fileName);
        }

        public bool IsValidUkprn(string fileName, long ukprn)
        {
            var matches = _fileNameRegex.Match(fileName);
            var fileUkprn = long.Parse(matches.Groups[2].Value);

            return fileUkprn == ukprn;
        }
    }
}
