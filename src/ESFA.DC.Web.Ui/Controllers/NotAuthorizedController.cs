using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class NotAuthorizedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}