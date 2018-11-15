using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.JobStatus.Interface;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissonHistoryViewModel
    {
        public string JobType { get; set; }

        public string FileName { get; set; }

        public string DateTimeSubmitted { get; set; }

        public long JobId { get; set; }

        public JobStatusType Status { get; set; }

        public string ReportsFileName { get; set; }
    }
}
