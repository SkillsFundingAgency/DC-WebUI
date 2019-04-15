using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ReportHistoryViewModel
    {
        public ReportHistoryViewModel()
        {
            RelatedJobs = new Dictionary<EnumJobType, long>();
        }
        public string ReportFileName { get; set; }

        public decimal CombinedFileSize { get; set; }

        public Dictionary<EnumJobType, long> RelatedJobs { get; set; }

        public int PeriodNumber { get; set; }

        public int AcademicYear { get; set; }
        public string DisplayCollectionYear { get; set; }

        public long Ukprn { get; set; }
    }
}
