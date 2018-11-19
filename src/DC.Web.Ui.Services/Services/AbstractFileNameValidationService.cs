using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public abstract class AbstractFileNameValidationService : IFileNameValidationService
    {
        private readonly IKeyValuePersistenceService _persistenceService;
        private readonly FeatureFlags _featureFlags;

        protected AbstractFileNameValidationService(IKeyValuePersistenceService persistenceService, FeatureFlags featureFlags)
        {
            _persistenceService = persistenceService;
            _featureFlags = featureFlags;
        }

        protected abstract IEnumerable<string> FileNameExtensions { get; }

        protected abstract Regex FileNameRegex { get; }

        public abstract Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName);

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
    }
}
