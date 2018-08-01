using DC.Web.Ui.Base;
using ESFA.DC.DateTime.Provider.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    public class ConfirmationController : BaseController
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConfirmationController(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}