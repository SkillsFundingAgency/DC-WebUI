using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class InProgressController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.AutoRefresh = true;
            return View();
        }
    }
}