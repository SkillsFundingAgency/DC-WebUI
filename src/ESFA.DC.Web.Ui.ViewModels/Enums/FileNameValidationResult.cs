using System.ComponentModel;

namespace ESFA.DC.Web.Ui.ViewModels.Enums
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
        InvalidFileExtension = 40,
        [Description("This file already exists")]
        FileAlreadyExists = 50
    }
}
