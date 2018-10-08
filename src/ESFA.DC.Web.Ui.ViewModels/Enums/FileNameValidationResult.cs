using System.ComponentModel;

namespace ESFA.DC.Web.Ui.ViewModels.Enums
{
    public enum FileNameValidationResult
    {
        Valid = 1,
        EmptyFile = 10,
        UkprnDifferentToFileName = 20,
        InvalidFileNameFormat = 30,
        InvalidFileExtension = 40,
        FileAlreadyExists = 50,
        InvalidYear = 60
    }
}
