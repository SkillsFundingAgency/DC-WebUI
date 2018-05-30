using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace DC.Web.Ui.Services.ValidationErrors
{
    public interface IValidationErrorsService
    {
        Task<IEnumerable<ValidationErrorDto>> GetValidationErrors(long ukprn, long jobId);
    }
}
