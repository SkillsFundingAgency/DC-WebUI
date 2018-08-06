using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IFileNameValidationService
    {
        FileNameValidationResult ValidateFileName(string fileName, long? fileSize, long ukprn);

        bool IsValidExtension(string fileName);

        bool IsValidRegex(string fileName);

        bool IsValidUkprn(string fileName, long ukprn);
    }
}
