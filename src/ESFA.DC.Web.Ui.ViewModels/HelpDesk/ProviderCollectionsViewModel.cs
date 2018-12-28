﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels.HelpDesk
{
    public class ProviderCollectionsViewModel
    {
        public long Ukprn { get; set; }

        public string Name { get; set; }

        public int NumberOfContracts{ get; set; }

        public IEnumerable<SubmissionOptionViewModel> SubmissionOptionViewModels { get; set; }
    }
}
