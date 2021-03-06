﻿using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Jobs.Model.Enums;

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

        public string SubmittedBy { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }

        public long Ukprn { get; set; }

        public int PeriodNumber { get; set; }

        public string PeriodName { get; set; }

        public string EsfPeriodName { get; set; }


    }
}
