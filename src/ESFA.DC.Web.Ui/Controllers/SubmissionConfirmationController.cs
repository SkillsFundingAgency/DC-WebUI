using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-confirmation")]
    public class SubmissionConfirmationController : BaseController
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionConfirmationController(ISubmissionService submissionService, ILogger logger)
            : base(logger)
        {
            _submissionService = submissionService;
        }

        [HttpGet]
        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            var data = await _submissionService.GetConfirmation(Ukprn, jobId);
            return View(data);
        }
    }
}