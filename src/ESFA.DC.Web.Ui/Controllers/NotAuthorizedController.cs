﻿using System;
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
            if (User.IsHelpDeskUser())
            {
                return RedirectToAction("Index", "ProviderSearch", new { area = AreaNames.HelpDesk });
            }

            return View();
        }
    }
}