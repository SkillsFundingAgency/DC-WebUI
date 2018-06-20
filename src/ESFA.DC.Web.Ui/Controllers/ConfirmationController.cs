using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.DateTime.Provider.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    public class ConfirmationController : BaseController
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConfirmationController(AuthenticationSettings authenticationSettings, IDateTimeProvider dateTimeProvider)
            : base(authenticationSettings)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IActionResult Index()
        {
           //// IlrFileViewModel ilrSubmission = null;
           // var tempData = TempData["ilrSubmission"];
           // if (tempData != null)
           // {
           //     ilrSubmission = JsonConvert.DeserializeObject<IlrFileViewModel>(tempData.ToString());
           //     ilrSubmission.SubmissionDateTime = _dateTimeProvider.ConvertUtcToUk(ilrSubmission.SubmissionDateTime);
           // }

            return View(null);
        }
    }
}