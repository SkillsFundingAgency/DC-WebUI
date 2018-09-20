using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class ValidationResultsService : IValidationResultsService
    {
        private readonly IJsonSerializationService _serializationService;
        private readonly IReportService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;

        public ValidationResultsService(
            IJsonSerializationService serializationService,
            IReportService reportService,
            IDateTimeProvider dateTimeProvider,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings)
        {
            _serializationService = serializationService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
            _httpClient = httpClient;
            _baseUrl = apiSettings?.JobQueueBaseUrl;
        }

        public async Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var ilrValidationResult = await GetValidationResultsData(ukprn, jobId);
            if (ilrValidationResult == null)
            {
                return null;
            }

            return new ValidationResultViewModel()
            {
                JobId = jobId,
                TotalErrors = ilrValidationResult.TotalErrors,
                TotalErrorLearners = ilrValidationResult.TotalErrorLearners,
                TotalWarningLearners = ilrValidationResult.TotalWarningLearners,
                TotalWarnings = ilrValidationResult.TotalWarnings,
                TotalLearners = ilrValidationResult.TotalLearners,
                ReportFileSize = (await GetFileSize(ukprn, jobId, dateTimeUtc)).ToString("N1")
            };
        }

        public async Task<IlrValidationResult> GetValidationResultsData(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/ValidationResults/{ukprn}/{jobId}");

            if (!string.IsNullOrEmpty(data))
            {
                return _serializationService.Deserialize<IlrValidationResult>(data);
            }

            return null;
        }

        public string GetStorageFileName(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var reportFileName = "{0}/{1}/Validation Errors Report {2}";
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(reportFileName, ukprn, jobId, jobDateTime);
        }

        public string GetReportFileName(DateTime dateTimeUtc)
        {
            var reportFileName = "Validation Errors Report {0}";
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(reportFileName, jobDateTime);
        }

        public async Task<decimal> GetFileSize(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var fileName = $"{GetStorageFileName(ukprn, jobId, dateTimeUtc)}.csv";
            return await _reportService.GetReportFileSizeAsync(fileName);
        }
    }
}
