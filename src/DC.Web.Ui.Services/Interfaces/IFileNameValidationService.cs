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

        FileNameValidationResultViewModel ValidateExtension(string fileName, string errorMessage);

        FileNameValidationResultViewModel ValidateUkprn(string fileName, long ukprn);

        Task<FileNameValidationResultViewModel> ValidateUniqueFileAsync(string fileName, long ukprn);
    }
}
