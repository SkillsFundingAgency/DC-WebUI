using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Base
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected long Ukprn => User.Ukprn();
    }
}
