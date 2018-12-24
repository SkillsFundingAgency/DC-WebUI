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
        private readonly ICollectionManagementService _collectionManagementService;

        public ProviderDetailsController(ICollectionManagementService collectionManagementService)
        {
            _collectionManagementService = collectionManagementService;
        }

        [HttpGet]
        [Route("{ukprn}")]
        public async Task<IActionResult> Index(long ukprn)
        {
            var collectionTypes = await _collectionManagementService.GetSubmssionOptionsAsync(ukprn);
            var esfContracts = await _collectionManagementService.GetNumberOfEsfContracts(ukprn);

            return View();
        }
    }
}