using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization;
using DC.Web.Ui.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Route(AreaNames.Admin + "/")]
    public class SearchController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}