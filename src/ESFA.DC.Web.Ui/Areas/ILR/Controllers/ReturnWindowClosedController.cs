using DC.Web.Ui.Base;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    public class ReturnWindowClosedController : BaseController
    {
        public ReturnWindowClosedController(ILogger logger)
            : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}