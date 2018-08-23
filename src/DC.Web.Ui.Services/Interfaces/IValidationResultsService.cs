using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Reports.ValidationReport;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IValidationResultsService
    {
        Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, DateTime dateTimeUtc);

        string GetStorageFileName(long ukprn, long jobId, DateTime dateTimeUtc);

        string GetReportFileName(DateTime dateTimeUtc);

        Task<IlrValidationResult> GetValidationResultsData(long ukprn, long jobId);

        Task<decimal> GetFileSize(long ukprn, long jobId, DateTime dateTimeUtc);
    }
}