using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;
using ESFA.DC.ILR.ValidationErrorReport.Model;

namespace DC.Web.Ui.Services.ValidationErrors
{
    public interface IValidationErrorsService
    {
        Task<IEnumerable<ReportValidationError>> GetValidationErrors(long ukprn, long jobId);
    }
}
