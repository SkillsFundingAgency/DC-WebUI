using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public enum FileNameValidationResult
    {
        Valid = 1,
        [Description("Check file you want to upload")]
        EmptyFile = 10,
        [Description("Logged in user ukprn does not match file ukprn")]
        UkprnDifferentToFileName = 20,
        [Description("file name is invalid")]
        InvalidFileNameFormat = 30,
        [Description("Your file must be in an XML or Zip format")]
        InvalidFileExtension = 40
    }
}
