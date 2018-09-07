using System;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionMessageViewModel
    {
        public string CollectionName { get; set; }

        public int Period { get; set; }

        public string FileName { get; set; }

        public decimal FileSizeBytes { get; set; }

        public string SubmittedBy { get; set; }

        public long Ukprn { get; set; }

        public string NotifyEmail { get; set; }
    }
}