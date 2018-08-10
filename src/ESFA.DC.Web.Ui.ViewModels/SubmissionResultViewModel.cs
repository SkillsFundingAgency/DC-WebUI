using System;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class SubmissionResultViewModel
    {
        public long JobId { get; set; }

        public int PeriodNumber { get; set; }

        public string PeriodName { get; set; }

        public long FileSize { get; set; }
    }
}
