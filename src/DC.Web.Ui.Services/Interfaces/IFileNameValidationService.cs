using System.Threading.Tasks;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IFileNameValidationService
    {
        Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName);

        FileNameValidationResultViewModel ValidateExtension(string extension, string errorMessage);

        FileNameValidationResultViewModel ValidateLoggedInUserUkprn(string fileName, long ukprn);

        Task<FileNameValidationResultViewModel> ValidateUniqueFileAsync(string fileName, long ukprn);
    }
}
