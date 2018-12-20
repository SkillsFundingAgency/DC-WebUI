using System.Collections.Generic;

namespace ESFA.DC.Web.Ui.ViewModels.HelpDesk
{
    public class ProviderSearchResultViewModel
    {
        public ProviderSearchResultViewModel()
        {
            ProvidersList = new List<ProviderDetailViewModel>(); 
        }

        public string SearchTerm { get; set; }

        public List<ProviderDetailViewModel> ProvidersList { get; set; }
    }
}
