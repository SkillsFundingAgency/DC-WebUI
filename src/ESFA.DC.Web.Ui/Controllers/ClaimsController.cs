using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ClaimsController : BaseController
    {
        public ClaimsController(ILogger logger)
            : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}