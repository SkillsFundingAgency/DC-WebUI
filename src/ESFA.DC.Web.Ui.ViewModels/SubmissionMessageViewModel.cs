using System;
using System.Runtime.InteropServices.ComTypes;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionMessageViewModel
    {
        public SubmissionMessageViewModel(JobType jobType, long ukprn, long upin)
        {
            JobType = jobType;
            Ukprn = ukprn;
            Upin = upin;
        }

        public string CollectionName { get; set; }

        public int Period { get; set; }

        public string FileName { get; set; }

        public decimal FileSizeBytes { get; set; }

        public string SubmittedBy { get; set; }

        public long Ukprn { get; set; }

        public string NotifyEmail { get; set; }

        public JobType JobType { get; }

        public string StorageReference { get; set; }

        public long Upin { get; set; }
    }
}