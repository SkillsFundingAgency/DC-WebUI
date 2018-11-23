using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public class IlrFileNameValidationService : AbstractFileNameValidationService
    {
        private readonly IJobService _jobService;

        public IlrFileNameValidationService(
            [KeyFilter(JobType.IlrSubmission)]IKeyValuePersistenceService persistenceService,
            FeatureFlags featureFlags,
            IJobService jobService)
            : base(persistenceService, featureFlags)
        {
            _jobService = jobService;
        }

        protected override Regex FileNameRegex => new Regex("^(ILR)-([1-9][0-9]{7})-([0-9]{4})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))-([0-9]{2}).((XML)|(ZIP)|(xml)|(zip))$", RegexOptions.Compiled);

        protected override IEnumerable<string> FileNameExtensions => new List<string>() { ".zip", ".xml", ".ZIP", ".XML" };

        public override async Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName)
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            result = ValidateExtension(fileName, "Your file must be in an XML or Zip format");
            if (result != null)
            {
                return result;
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

            if (!IsValidSerialNumber(fileName))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidSerialNumber,
                    FieldError = "Serial number in the filename must not be 00",
                    SummaryError = "Serial number in the filename must not be 00"
                };
            }

            var fileExtension = fileName.Substring(fileName.Length - 4);
            result = ValidateRegex(fileName, $"File name should use the format ILR-LLLLLLLL-YYYY-yyyymmdd-hhmmss-NN{fileExtension}");
            if (result != null)
            {
                return result;
            }

            result = ValidateUkprn(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            result = await ValidateUniqueFileAsync(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            if (LaterFileExists(ukprn, fileName, collectionName))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.LaterFileAlreadySubmitted,
                    FieldError = "The date/time of the file is earlier than a previous transmission for this collection",
                    SummaryError = "The date/time of the file is earlier than a previous transmission for this collection"
                };
            }

            return new FileNameValidationResultViewModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

        public bool IsValidSerialNumber(string fileName)
        {
            if (!IsValidRegex(fileName))
            {
                return true;
            }

            var matches = FileNameRegex.Match(fileName);
            return matches.Groups[12].Value != "00";
        }

        public bool IsValidYear(string fileName)
        {
            if (!IsValidRegex(fileName))
            {
                return true;
            }

            var matches = FileNameRegex.Match(fileName);
            return matches.Groups[3].Value == "1819";
        }

        public bool LaterFileExists(long ukprn, string fileName, string collectionName)
        {
            var job = _jobService.GetLatestJob(ukprn, collectionName).Result;
            if (job == null || job.JobId == 0)
            {
                return false;
            }

            if (!IsValidRegex(fileName))
            {
                return true;
            }

            var fileDateTime = GetDateTime(fileName);
            var existingJobFileDateTime = GetDateTime(job.FileName.Split('/')[1]);
            return fileDateTime < existingJobFileDateTime;
        }

        private DateTime GetDateTime(string fileName)
        {
            var matches = FileNameRegex.Match(fileName);

            return DateTime.ParseExact(
                $"{matches.Groups[4].Value}-{matches.Groups[8].Value}",
                "yyyyMMdd-HHmmss",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
