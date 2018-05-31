﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Web.Ui.ViewModels
{
    public class ValidationErrorViewModel
    {
        public string ErrorMessage { get; set; }

        public string LearnRefNumber { get; set; }

        public string FieldValues { get; set; }

        public string Severity { get; set; }

        public int AimSequenceNumber { get; set; }

        public string RuleId { get; set; }
    }
}
