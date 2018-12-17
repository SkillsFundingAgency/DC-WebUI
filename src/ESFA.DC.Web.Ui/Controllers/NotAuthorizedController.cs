using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class NotAuthorizedController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsAdminUser())
            {
                return RedirectToAction("Index", "Search", new { area = AreaNames.Admin });
            }

            return View();
        }
    }
}