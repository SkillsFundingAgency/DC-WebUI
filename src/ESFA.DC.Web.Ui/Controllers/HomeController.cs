﻿using DC.Web.Authorization.Idams;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsHelpDeskUser())
                {
                    return RedirectToAction("Index", "ProviderSearch", new { area = AreaNames.HelpDesk });
                }

                return RedirectToAction("Index", "SubmissionOptionsAuthorised");
            }

            return View();
        }
    }
}