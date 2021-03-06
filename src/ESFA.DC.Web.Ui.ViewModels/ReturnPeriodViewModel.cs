﻿using System;
using System.Globalization;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ReturnPeriodViewModel
    {
        private readonly string _periodName;

        public ReturnPeriodViewModel(int periodNumber)
        {
            PeriodNumber = periodNumber;
            _periodName = $"R{periodNumber.ToString("00", NumberFormatInfo.InvariantInfo)}";
        }

        public int PeriodNumber { get; set; }

        public string PeriodName() => _periodName;

        public string NextOpeningDate { get; set; }

        public int DaysToClose { get; set; }

        public string PeriodCloseDate { get; set; }
    }
}
