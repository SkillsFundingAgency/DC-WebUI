using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Services.ValidationErrors
{
    public interface IValidationErrorsService
    {
        Task<IEnumerable<ValidationError>> GetValidationErrors(long ukprn, long jobId);
    }
}
