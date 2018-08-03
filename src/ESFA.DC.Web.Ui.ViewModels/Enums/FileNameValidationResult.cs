using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public enum FileNameValidationResult
    {
        Valid = 1,
        UkprnDifferentToFileName=2,
        InvalidFileNameFormat =3
    }
}
