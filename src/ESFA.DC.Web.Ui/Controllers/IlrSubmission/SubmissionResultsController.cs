using DC.Web.Ui.Base;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("submission-results")]
    public class SubmissionResultsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}