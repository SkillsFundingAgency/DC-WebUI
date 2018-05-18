using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.ValidationErrors;
using DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ValidationErrorsController : Controller
    {
        private readonly IValidationErrorsService _validationErrorsService;

        public ValidationErrorsController(IValidationErrorsService validationErrorsService)
        {
            _validationErrorsService = validationErrorsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _validationErrorsService.GetValidationErrors(0));
        }
    }
}