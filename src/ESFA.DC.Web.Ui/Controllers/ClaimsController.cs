using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ClaimsController : BaseController
    {
       public IActionResult Index()
        {
            return View();
        }
    }
}