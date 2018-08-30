using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IFileNameValidationService
    {
        Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn);

        bool IsValidExtension(string fileName);

        bool IsValidRegex(string fileName);

        bool IsValidUkprn(string fileName, long ukprn);

        Task<bool> IsUniqueFileAsync(string fileName);
    }
}
