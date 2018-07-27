using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Amqp.Framing;

namespace DC.Web.Ui.Services.Models
{
    public class ReturnPeriod
    {
        private readonly int _periodNumber;

        public ReturnPeriod(string periodName)
        {
            PeriodName = periodName;
            int.TryParse(PeriodName?.Replace("R", "0"), out _periodNumber);
        }

        public string PeriodName { get; set; }

        public int PeriodNumber() => _periodNumber;
    }
}
