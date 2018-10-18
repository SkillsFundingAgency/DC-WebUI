﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public class IlrFileNameValidationService : IFileNameValidationService
    {
        private readonly IKeyValuePersistenceService _persistenceService;
        private readonly Regex _fileNameRegex = new Regex("^(ILR)-([1-9][0-9]{7})-(1819)-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-(([1-9][0-9])|(0[1-9])).((XML)|(ZIP)|(xml)|(zip))$", RegexOptions.Compiled);

        public IlrFileNameValidationService([KeyFilter(JobType.IlrSubmission)]IKeyValuePersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
        }

        public async Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn)
        {
            var result = new FileNameValidationResultViewModel();
            if (string.IsNullOrEmpty(fileName) || fileSize == null || fileSize.Value == 0)
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.EmptyFile,
                    FieldError = "Choose a file to upload",
                    SummaryError = "Check file you want to upload"
                };
            }

            if (!IsValidExtension(fileName))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = "Your file must be in an XML or Zip format",
                    SummaryError = "Your file must be in an XML or Zip format"
                };
            }

            if (!IsValidRegex(fileName))
            {
                var fileExtension = fileName.Substring(fileName.Length - 4);
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileNameFormat,
                    FieldError = $"File name should use the format ILR-LLLLLLLL-YYYY-yyyymmdd-hhmmss-NN{fileExtension}",
                    SummaryError = $"File name should use the format ILR-LLLLLLLL-YYYY-yyyymmdd-hhmmss-NN{fileExtension}"
                };
            }

            if (!IsValidUkprn(fileName, ukprn))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileNameFormat,
                    FieldError = "The UKPRN in the filename does not match the UKPRN associated with your IdAMS account",
                    SummaryError = "The UKPRN in the filename does not match the UKPRN associated with your IdAMS account"
                };
            }

            if (!IsValidYear(fileName))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidYear,
                    FieldError = "The year in the filename should match the current year",
                    SummaryError = "The year in the filename should match the current year"
                };
            }

            //if (!(await IsUniqueFileAsync(fileName)))
            //{
            //    return FileNameValidationResult.FileAlreadyExists;
            //}

            return new FileNameValidationResultViewModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
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
            long.TryParse(matches.Groups[2].Value, out var fileUkprn);
            return fileUkprn == ukprn;
        }

        public bool IsValidYear(string fileName)
        {
            var matches = _fileNameRegex.Match(fileName);
            int.TryParse(matches.Groups[5].Value, out var year);
            return year == DateTime.Now.Year;
        }

        public async Task<bool> IsUniqueFileAsync(string fileName)
        {
            return !(await _persistenceService.ContainsAsync(fileName));
        }
    }
}
