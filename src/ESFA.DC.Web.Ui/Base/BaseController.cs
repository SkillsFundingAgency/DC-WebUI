using DC.Web.Ui.Extensions;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Base
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        private readonly AuthenticationSettings _authenticationSettings;

        protected BaseController(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        protected long Ukprn => _authenticationSettings.Enabled == false ? 9999 : User.Ukprn();
    }
}
