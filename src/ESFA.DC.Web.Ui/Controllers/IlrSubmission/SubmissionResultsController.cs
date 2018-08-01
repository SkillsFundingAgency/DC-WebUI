using DC.Web.Ui.Base;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("submission-results")]
    public class SubmissionResultsController : BaseController
    {
        public SubmissionResultsController(ILogger logger)
            : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}