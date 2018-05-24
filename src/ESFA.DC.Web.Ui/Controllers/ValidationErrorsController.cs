using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.ValidationErrors;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ValidationErrorsController : BaseController
    {
        private readonly IValidationErrorsService _validationErrorsService;

        public ValidationErrorsController(IValidationErrorsService validationErrorsService, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _validationErrorsService = validationErrorsService;
        }

        public async Task<IActionResult> Index(long jobId)
        {
            return View(await _validationErrorsService.GetValidationErrors(Ukprn, jobId));
        }
    }
}