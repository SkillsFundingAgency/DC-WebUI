using System;
using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ProviderHistoryViewModel
    {
        public List<SubmissonHistoryViewModel> SubmissionItems { get; set; }
        
        public List<ReportHistoryViewModel> ReportHistoryItems { get; set; }

        public List<int> Periods { get; set; }

        public List<string> CollectionTypes { get; set; }

        public List<string> CollectionTypeFiltersList { get; set; }

        public List<int> AcademicYears { get; set; }

        public List<int> AcademicYearFiltersList { get; set; }
    }
}
