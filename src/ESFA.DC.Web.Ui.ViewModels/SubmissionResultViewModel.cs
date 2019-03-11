using System;
using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionResultViewModel
    {
        public List<SubmissonHistoryViewModel> SubmissionItems { get; set; }
        
        public List<ReportHistoryViewModel> ReportHistoryItems { get; set; }

        public List<int> Periods { get; set; }

        public List<string> CollectionTypes { get; set; }

        public List<string> JobTypeFiltersList { get; set; }
    }
}
