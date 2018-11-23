using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services.Enums;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class ValidationResultsService : IValidationResultsService
    {
        private readonly IJsonSerializationService _serializationService;
        private readonly IStorageService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;

        public ValidationResultsService(
            IJsonSerializationService serializationService,
            IStorageService reportService,
            IDateTimeProvider dateTimeProvider,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings)
        {
            _serializationService = serializationService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
            _httpClient = httpClient;
            _baseUrl = apiSettings?.JobManagementApiBaseUrl;
        }

        public async Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, JobType jobType, DateTime dateTimeUtc)
        {
            var validationResult = await GetValidationResultsData(ukprn, jobId);
            if (validationResult == null)
            {
                return null;
            }

            var dataMatchSize = await GetFileSize(ukprn, jobId, jobType, dateTimeUtc, ValidationResultsReportType.DataMatch);

            return new ValidationResultViewModel()
            {
                JobId = jobId,
                TotalErrors = validationResult.TotalErrors,
                TotalErrorLearners = validationResult.TotalErrorLearners,
                TotalWarningLearners = validationResult.TotalWarningLearners,
                TotalWarnings = validationResult.TotalWarnings,
                TotalLearners = validationResult.TotalLearners,
                ReportFileSize = (await GetFileSize(ukprn, jobId, jobType, dateTimeUtc, ValidationResultsReportType.DetailedErrors)).ToString("N1"),
                ErrorMessage = validationResult.ErrorMessage,
                DataMatchReportFileSize = dataMatchSize.ToString("N1"),
                HasDataMatchReport = dataMatchSize > 0,
                TotalDataMatchErrors = validationResult.TotalDataMatchErrors,
                TotalDataMatchLearners = validationResult.TotalDataMatchLearners
            };
        }

        public async Task<FileValidationResult> GetValidationResultsData(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/ValidationResults/{ukprn}/{jobId}");

            if (!string.IsNullOrEmpty(data))
            {
                return _serializationService.Deserialize<FileValidationResult>(data);
            }

            return null;
        }

        public string GetStorageFileName(long ukprn, long jobId, DateTime dateTimeUtc, ValidationResultsReportType whichReport)
        {
            var reportFileName = whichReport == ValidationResultsReportType.DetailedErrors ? "{0}/{1}/Rule Violation Report {2}" : "{0}/{1}/Apprenticeship Data Match Report {2}";
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(reportFileName, ukprn, jobId, jobDateTime);
        }

        public string GetReportFileName(DateTime dateTimeUtc, ValidationResultsReportType whichReport)
        {
            var reportFileName = whichReport == ValidationResultsReportType.DetailedErrors ? "Rule Violation Report {0}" : "Apprenticeship Data Match Report {0}";
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(reportFileName, jobDateTime);
        }

        public async Task<decimal> GetFileSize(long ukprn, long jobId, JobType jobType, DateTime dateTimeUtc, ValidationResultsReportType whichReport)
        {
            var fileName = $"{GetStorageFileName(ukprn, jobId, dateTimeUtc, whichReport)}.csv";
            return await _reportService.GetReportFileSizeAsync(fileName, jobType);
        }
    }
}
