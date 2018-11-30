using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public abstract class AbstractFileNameValidationService : IFileNameValidationService
    {
        private readonly IKeyValuePersistenceService _persistenceService;
        private readonly FeatureFlags _featureFlags;
        private readonly IJobService _jobService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _apiBaseUrl;

        protected AbstractFileNameValidationService(
            IKeyValuePersistenceService persistenceService,
            FeatureFlags featureFlags,
            IJobService jobService,
            IDateTimeProvider dateTimeProvider,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings)
        {
            _persistenceService = persistenceService;
            _featureFlags = featureFlags;
            _jobService = jobService;
            _dateTimeProvider = dateTimeProvider;
            _httpClient = httpClient;
            _apiBaseUrl = apiSettings?.JobManagementApiBaseUrl;
        }

        protected abstract IEnumerable<string> FileNameExtensions { get; }

        protected abstract Regex FileNameRegex { get; }

        public abstract Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName);

        public abstract DateTime GetFileDateTime(string fileName);

        public FileNameValidationResultViewModel ValidateEmptyFile(string fileName, long? fileSize)
        {
            if (string.IsNullOrEmpty(fileName) || fileSize == null || fileSize.Value == 0)
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.EmptyFile,
                    FieldError = "Choose a file to upload",
                    SummaryError = "Check file you want to upload"
                };
            }

            return null;
        }

        public FileNameValidationResultViewModel ValidateExtension(string fileName, string errorMessage)
        {
            var fileExtension = fileName.Substring(Math.Max(0, fileName.Length - 4));
            if (!FileNameExtensions.Contains(fileExtension))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }

        public FileNameValidationResultViewModel ValidateUkprn(string fileName, long ukprn)
        {
            var matches = FileNameRegex.Match(fileName);
            long fileUkprn = 0;
            if (matches.Groups.Count > 2)
            {
                long.TryParse(matches.Groups[2].Value, out fileUkprn);
            }

            if (fileUkprn != ukprn)
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileNameFormat,
                    FieldError = "The UKPRN in the filename does not match the UKPRN associated with your IdAMS account",
                    SummaryError = "The UKPRN in the filename does not match the UKPRN associated with your IdAMS account"
                };
            }

            return null;
        }

        public FileNameValidationResultViewModel ValidateRegex(string fileName, string errorMessage)
        {
            if (!IsValidRegex(fileName))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }

        public bool IsValidRegex(string fileName)
        {
            return FileNameRegex.IsMatch(fileName);
        }

        public async Task<FileNameValidationResultViewModel> ValidateUniqueFileAsync(string fileName, long ukprn)
        {
            if (_featureFlags.DuplicateFileCheckEnabled)
            {
                if (await _persistenceService.ContainsAsync($"{ukprn}/{fileName}"))
                {
                    return new FileNameValidationResultViewModel()
                    {
                        ValidationResult = FileNameValidationResult.FileAlreadyExists,
                        FieldError =
                            "You have already uploaded a file with the same filename. Upload a file with a different filename",
                        SummaryError =
                            "You have already uploaded a file with the same filename. Upload a file with a different filename"
                    };
                }
            }

            return null;
        }

        public FileNameValidationResultViewModel LaterFileExists(long ukprn, string fileName, string collectionName)
        {
            var job = _jobService.GetLatestJob(ukprn, collectionName).Result;
            if (job == null || job.JobId == 0)
            {
                return null;
            }

            if (!IsValidRegex(fileName))
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
                    FieldError = "The date/time of the file is earlier than a previous transmission for this collection",
                    SummaryError = "The date/time of the file is earlier than a previous transmission for this collection"
                };
            }

            return null;
        }

        public async Task<bool> IsProviderValidForSubmission(params string[] parameters)
        {
            var parametersString = string.Join("/", parameters);

            try
            {
                await _httpClient.GetDataAsync($"{_apiBaseUrl}/file-validation/{parametersString}");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public FileNameValidationResultViewModel IsFileAfterCurrentDateTime(long ukprn, string fileName, string collectionName)
        {
            if (!IsValidRegex(fileName))
            {
                return null;
            }

            var fileDateTime = GetFileDateTime(fileName);
            if (fileDateTime > _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc()))
            {
                return new FileNameValidationResultViewModel()
                {
                    ValidationResult = FileNameValidationResult.EarlierThanTodayFileSubmitted,
                    FieldError = "The date and time in the filename must not be later than today’s date and time",
                    SummaryError = "The date and time in the filename must not be later than today’s date and time"
                };
            }

            return null;
        }
    }
}
