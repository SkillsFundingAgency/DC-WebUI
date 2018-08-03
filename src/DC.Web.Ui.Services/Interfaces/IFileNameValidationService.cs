using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IFileNameValidationService
    {
        FileNameValidationResult ValidateFile(string fileName, long ukprn);
    }
}
