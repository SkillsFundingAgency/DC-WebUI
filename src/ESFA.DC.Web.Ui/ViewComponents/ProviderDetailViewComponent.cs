using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.ViewComponents
{
    public class ProviderDetailViewComponent : ViewComponent
    {
        private readonly IProviderService _providerService;

        public ProviderDetailViewComponent(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public async Task<IViewComponentResult> InvokeAsync(long ukprn)
        {
            var providerResult = await _providerService.GetProviderDetails(ukprn);

            return View(providerResult);
        }
    }
}
