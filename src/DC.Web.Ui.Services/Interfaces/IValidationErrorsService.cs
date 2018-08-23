using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IValidationErrorsService
    {
        Task<ValidationResultViewModel> GetValidationResult(long ukprn, long jobId, DateTime dateTimeUtc);

        string GetStorageFileName(long ukprn, long jobId, DateTime dateTimeUtc);

        string GetReportFileName(DateTime dateTimeUtc);
    }
}