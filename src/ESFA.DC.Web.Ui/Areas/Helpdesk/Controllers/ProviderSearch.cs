using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.Controllers
{
    [Area(AreaNames.HelpDesk)]
    [Route(AreaNames.HelpDesk + "/")]
    public class ProviderSearch : BaseHelpDeskController
    {
        private readonly IProviderService _providerService;

        public ProviderSearch(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View();
            }

            return await GetResults(searchTerm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetResults(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "You haven't entered any provider details. Please enter the provider name or UKPRN");
                ModelState.AddModelError(ErrorMessageKeys.HelpDesk_SearchProvider, "You haven't entered any provider details. Please enter the provider name or UKPRN");
                return View("Index");
            }

            var result = await _providerService.GetSearchResults(searchTerm);

            return View("Results", result);
        }
    }
}