using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.ValidationErrors;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class InProgressController : Controller
    {
        private readonly IValidationErrorsService _validationErrorsService;

        public InProgressController(IValidationErrorsService validationErrorsService)
        {
            _validationErrorsService = validationErrorsService;
        }

        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.AutoRefresh = true;
            var valErrors = await _validationErrorsService.GetValidationErrors(User.Ukprn(), jobId);
            if (valErrors == null)
            {
                return View();
            }

            return RedirectToAction("Index", "ValidationResults", new { jobId });
        }
    }
}