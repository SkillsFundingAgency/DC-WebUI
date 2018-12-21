using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels.HelpDesk
{
    public class ProviderDetailViewModel
    {
        public string Name { get; set; }

        public long Ukprn { get; set; }

        public string LastSubmittedBy { get; set; }

        public string LastSubmittedDate { get; set; }

        public string LastSubmittedByEmail { get; set; }
    }
}
