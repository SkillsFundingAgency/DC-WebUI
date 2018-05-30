using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    public class ConfirmationController : BaseController
    {
        public ConfirmationController(AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
        }

        public IActionResult Index()
        {
            IlrFileViewModel ilrSubmission = null;
            var tempData = TempData["ilrSubmission"];
            if (tempData != null)
            {
                ilrSubmission = JsonConvert.DeserializeObject<IlrFileViewModel>(tempData.ToString());
            }

            return View(ilrSubmission);
        }
    }
}