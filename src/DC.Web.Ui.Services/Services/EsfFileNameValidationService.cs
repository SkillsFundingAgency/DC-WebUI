﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public class EsfFileNameValidationService : AbstractFileNameValidationService
    {
        private readonly IJobService _jobService;

        public EsfFileNameValidationService(
            [KeyFilter(EnumJobType.EsfSubmission)]IKeyValuePersistenceService persistenceService,
            FeatureFlags featureFlags,
            IJobService jobService,
            IDateTimeProvider dateTimeProvider,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings)
            : base(persistenceService, featureFlags, jobService, dateTimeProvider, httpClient, apiSettings)
        {
            _jobService = jobService;
        }

        protected override Regex FileNameRegex => new Regex(@"^(?i)(SUPPDATA)-([1-9][0-9]{7})-([0-9a-zA-Z-]{1,20})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9]))(\.csv)$", RegexOptions.Compiled);

        protected override IEnumerable<string> FileNameExtensions => new List<string>() { ".CSV" };

        public override async Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName)
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            string ext = Path.GetExtension(fileName);
            result = ValidateExtension(ext, "Your file must be in a CSV format");
            if (result != null)
            {
                return result;
            }

            result = ValidateRegex(fileName, "File name should use the format SUPPDATA-LLLLLLLL-CCCCCCCCCCCCCCCCCCCC-yyyymmdd-hhmmss.csv");
            if (result != null)
            {
                return result;
            }

            result = ValidateLoggedInUserUkprn(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            result = await ValidateUniqueFileAsync(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            result = await LaterFileExists(ukprn, fileName, collectionName);
            if (result != null)
            {
                return result;
            }

            result = IsFileAfterCurrentDateTime(ukprn, fileName, collectionName);
            if (result != null)
            {
                return result;
            }

            result = await ValidateOrganisation(ukprn);
            if (result != null)
            {
                return result;
            }

            result = await IsContractReferenceValid(ukprn, fileName);
            if (result != null)
            {
                return result;
            }

            return new FileNameValidationResultViewModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

        public override async Task<FileNameValidationResultViewModel> LaterFileExists(long ukprn, string fileName, string collectionName)
        {
            var matches = FileNameRegex.Match(fileName);
            var job = await _jobService.GetLatestJob(ukprn, matches.Groups[3].Value, collectionName);
            if (job == null || job.JobId == 0)
            {
                return null;
            }

            var fileDateTime = GetFileDateTime(fileName);
            var existingJobFileDateTime = GetFileDateTime(job.FileName.Split('/')[1]);
            if (fileDateTime < existingJobFileDateTime)
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.LaterFileAlreadySubmitted,
                    FieldError = "The date/time of the file must be greater than previous transmission with the same ConRefNumber and UKPRN",
                    SummaryError = "The date/time of the file must be greater than previous transmission with the same ConRefNumber and UKPRN"
                };
            }

            return null;
        }

        public async Task<FileNameValidationResultViewModel> IsContractReferenceValid(long ukprn, string fileName)
        {
            var matches = FileNameRegex.Match(fileName);
            var result = await IsProviderValidForSubmission("conref", ukprn.ToString(), matches.Groups[3].Value);

            if (!result)
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidContractRefNumber,
                    FieldError = "An approved contract does not exist with the specified ConRefNumber",
                    SummaryError = "An approved contract does not exist with the specified ConRefNumber"
                };
            }

            return null;
        }

        public override DateTime GetFileDateTime(string fileName)
        {
            var matches = FileNameRegex.Match(fileName);

            return DateTime.ParseExact(
                $"{matches.Groups[4].Value}-{matches.Groups[8].Value}",
                "yyyyMMdd-HHmmss",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
