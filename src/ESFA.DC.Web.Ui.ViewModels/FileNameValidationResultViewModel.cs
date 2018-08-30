using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class FileNameValidationResultViewModel
    {
        public FileNameValidationResult ValidationResult { get; set; }

        public string SummaryError { get; set; }

        public string FieldError { get; set; }
    }
}
