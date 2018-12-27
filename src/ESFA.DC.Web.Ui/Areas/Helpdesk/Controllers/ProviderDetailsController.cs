using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Web.Ui.ViewModels.HelpDesk;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.Controllers
{
    [Area(AreaNames.HelpDesk)]
    [Route(AreaNames.HelpDesk + "/provider")]
    public class ProviderDetailsController : BaseHelpDeskController
    {
        private readonly IProviderService _providerService;

        public ProviderDetailsController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpGet]
        [Route("{ukprn}/{searchTerm}")]
        public async Task<IActionResult> Index(long ukprn, string searchTerm)
        {
            var providerResult = await _providerService.GetProviderDetails(ukprn);

            return View(providerResult);
        }
    }
}