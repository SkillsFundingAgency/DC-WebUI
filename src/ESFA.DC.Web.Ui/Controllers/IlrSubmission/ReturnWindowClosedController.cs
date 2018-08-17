using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    public class ReturnWindowClosedController : BaseController
    {
        public ReturnWindowClosedController(ILogger logger)
            : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}