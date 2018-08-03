using System;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ValidationResultViewModel
    {
        public string Filename { get; set; }

       // [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss.fff tt}")]
        public DateTime SubmissionDateTime { get; set; }

        public long ReportFileSize { get; set; }

        public long JobId { get; set; }

        public string UploadedBy { get; set; }

        public int TotalLearners { get; set; }

        public int TotalErrors { get; set; }

        public int TotalWarnings { get; set; }

        public int TotalErrorLearners { get; set; }

        public int TotalWarningLearners { get; set; }
    }
}
