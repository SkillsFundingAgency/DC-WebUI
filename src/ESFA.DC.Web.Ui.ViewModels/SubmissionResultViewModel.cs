using System;
using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionResultViewModel
    {
        public int PeriodNumber { get; set; }

        public string PeriodName { get; set; }

        public List<SubmissonHistoryViewModel> CurrentPeriodSubmissions { get; set; }

        public List<SubmissonHistoryViewModel> PreviousPeriodSubmissions { get; set; }

        public List<ReportHistoryViewModel> ReportHistoryItems { get; set; }

        public string CollectionYearStart { get; set; }

        public string CollectionYearEnd { get; set; }


    }
}
