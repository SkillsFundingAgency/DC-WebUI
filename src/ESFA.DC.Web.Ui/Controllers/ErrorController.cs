using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError("Error occured in Web Ui", exceptionFeature.Error, callerFilePath: exceptionFeature.Path);
            }

            return View();
        }
    }
}