using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Admin.Controllers
{
    [Authorize(Policy = PolicyTypes.HelpDeskAccess)]
    public class BaseAdminController : Controller
    {
    }
}
