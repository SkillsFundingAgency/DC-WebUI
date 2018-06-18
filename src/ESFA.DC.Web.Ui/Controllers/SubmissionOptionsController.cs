using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-options")]
    public class SubmissionOptionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(SubmissionTypes submissionType)
        {
            if (!Enum.IsDefined(typeof(SubmissionTypes), submissionType))
            {
                //TODO: Display an error
            }

            switch (submissionType)
            {
                case SubmissionTypes.Ilr:
                    return RedirectToAction("Index", "ILRSubmission");
                case SubmissionTypes.Eas:
                    throw new Exception("Not implemented");
                default:
                    throw new Exception("unknown submittion type ");
            }
        }
    }
}