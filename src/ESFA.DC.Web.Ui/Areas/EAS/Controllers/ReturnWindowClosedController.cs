using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.EAS.Controllers
{
    [Area(AreaNames.Esf)]
    [Route(AreaNames.Esf + "/window-closed")]
    public class ReturnWindowClosedController : Controller
    {
        private readonly ICollectionManagementService _collectionManagementService;

        public ReturnWindowClosedController(ICollectionManagementService collectionManagementService)
        {
            _collectionManagementService = collectionManagementService;
        }

        [HttpGet]
        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName)
        {
            var period = await _collectionManagementService.GetNextPeriodAsync(collectionName);
            return View(period);
        }
    }
}