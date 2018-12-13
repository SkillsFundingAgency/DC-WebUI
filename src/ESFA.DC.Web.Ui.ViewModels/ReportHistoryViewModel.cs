using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ReportHistoryViewModel
    {
        public ReportHistoryViewModel()
        {
            RelatedJobs = new Dictionary<JobType, long>();
        }
        public string ReportFileName { get; set; }

        public decimal CombinedFileSize { get; set; }

        public Dictionary<JobType,long> RelatedJobs { get; set; }

        public int PeriodNumber { get; set; }

        public int CollectionYear { get; set; }
        public string DisplayCollectionYear { get; set; }
    }
}
