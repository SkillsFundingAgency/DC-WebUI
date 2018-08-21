using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Reports.ValidationReport;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class ValidationErrorsService : IValidationErrorsService
    {
        private readonly string _reportFileName = "{0}/{1}/Validation Errors Report {2}";
        private readonly IJsonSerializationService _serializationService;
        private readonly IReportService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IKeyValuePersistenceService _persistenceService;

        public ValidationErrorsService(
            IJsonSerializationService serializationService,
            IReportService reportService,
            IDateTimeProvider dateTimeProvider,
            IKeyValuePersistenceService persistenceService)
        {
            _serializationService = serializationService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
            _persistenceService = persistenceService;
        }

        public async Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var ilrValidationResult = await GetValidationErrorsData(ukprn, jobId, dateTimeUtc);
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
                ReportFileSize = await GetFileSize(ukprn, jobId, dateTimeUtc),
                ReportFileName = $"{GetFileName(ukprn, jobId, dateTimeUtc)}.csv"
        };
        }

        public async Task<IlrValidationResult> GetValidationErrorsData(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var validationErrorsKey = $"{GetFileName(ukprn, jobId, dateTimeUtc)}.json";
            var exists = await _persistenceService.ContainsAsync(validationErrorsKey);
            if (exists)
            {
                var data = await _persistenceService.GetAsync(validationErrorsKey);
                return _serializationService.Deserialize<IlrValidationResult>(data);
            }

            return null;
        }

        public string GetFileName(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(_reportFileName, ukprn, jobId, jobDateTime);
        }

        public async Task<long> GetFileSize(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var fileName = $"{GetFileName(ukprn, jobId, dateTimeUtc)}.csv";
            return await _reportService.GetReportFileSizeAsync(fileName);
        }
    }
}
