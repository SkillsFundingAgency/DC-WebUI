using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Ui.ViewModels.HelpDesk;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class ProviderSearchResultViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<ProviderDetailViewModel> ProvidersList { get; set; }
    }
}
