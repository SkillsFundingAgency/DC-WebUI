using DC.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.Controllers
{
    [Authorize(Policy = PolicyTypes.HelpDeskAccess)]
    public class BaseAdminController : Controller
    {
    }
}
