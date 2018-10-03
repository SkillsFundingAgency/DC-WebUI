using System;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionResultViewModel
    {
        public long JobId { get; set; }

        public int PeriodNumber { get; set; }

        public string PeriodName { get; set; }

        public string FileSize { get; set; }

        public JobStatusType Status { get; set; }

        public JobType JobType { get; set; }
    }
}
