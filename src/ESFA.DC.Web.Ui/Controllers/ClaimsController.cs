using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ClaimsController : BaseController
    {
        public ClaimsController(AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}