using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.HelpDesk;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IProviderService
    {
        Task<ProviderSearchResultViewModel> GetSearchResults(string searchTerm);
    }
}
