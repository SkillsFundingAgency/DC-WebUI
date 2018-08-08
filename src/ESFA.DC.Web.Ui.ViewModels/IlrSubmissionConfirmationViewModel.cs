using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class IlrSubmissionConfirmationViewModel
    {
        public long JobId { get; set; }

        public string FileName { get; set; }

        public string SubmittedBy { get; set; }

        public string SubmittedAt { get; set; }

        public string PeriodName { get; set; }

    }
}
