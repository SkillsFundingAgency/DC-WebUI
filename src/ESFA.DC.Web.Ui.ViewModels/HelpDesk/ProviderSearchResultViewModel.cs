using System.Collections.Generic;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.Web.Ui.ViewModels.HelpDesk
{
    public class ProviderSearchResultViewModel
    {
        public ProviderSearchResultViewModel()
        {
            ProvidersList = new List<ProviderDetail>(); 
        }

        public string SearchTerm { get; set; }

        public List<ProviderDetail> ProvidersList { get; set; }
    }
}
