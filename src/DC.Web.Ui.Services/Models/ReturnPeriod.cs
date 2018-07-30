using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Amqp.Framing;

namespace DC.Web.Ui.Services.Models
{
    public class ReturnPeriod
    {
        private readonly string _periodName;

        public ReturnPeriod(int periodNumber)
        {
            PeriodNumber = periodNumber;
            _periodName = $"R{periodNumber}";
        }

        public int PeriodNumber { get; set; }

        public string PeriodName() => _periodName;
    }
}
