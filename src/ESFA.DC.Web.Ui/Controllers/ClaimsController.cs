using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}