using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.Controllers
{
    [Area(AreaNames.HelpDesk)]
    [Route(AreaNames.HelpDesk + "/")]
    public class ProviderSearch : BaseAdminController
    {
        private readonly IProviderService _providerService;

        public ProviderSearch(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetResults(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "There is a problem");
                ModelState.AddModelError(ErrorMessageKeys.HelpDesk_SearchProvider, "Please enter a search term");
                return View("Index");
            }

            var result = await _providerService.GetSearchResults(searchTerm);

            return View("Results", result);
        }
    }
}