using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Services.Enums;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IValidationResultsService
    {
        Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, JobType jobType, DateTime dateTimeUtc);

        string GetStorageFileName(long ukprn, long jobId, DateTime dateTimeUtc, ValidationResultsReportType reportType);

        string GetReportFileName(DateTime dateTimeUtc, ValidationResultsReportType reportType);

        Task<FileValidationResult> GetValidationResultsData(long ukprn, long jobId);

        Task<decimal> GetFileSize(long ukprn, long jobId, JobType jobType, DateTime dateTimeUtc, ValidationResultsReportType reportType);
    }
}