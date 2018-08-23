using System;
using System.ComponentModel.DataAnnotations;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ValidationResultViewModel
    {
        public string ReportFileSize { get; set; }

        public long JobId { get; set; }

        public int TotalLearners { get; set; }

        public int TotalErrors { get; set; }

        public int TotalWarnings { get; set; }

        public int TotalErrorLearners { get; set; }

        public int TotalWarningLearners { get; set; }

        public string CollectionName { get; set; }
    }
}
