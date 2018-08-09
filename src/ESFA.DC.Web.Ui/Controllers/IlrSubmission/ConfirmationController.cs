using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("confirmation")]
    public class ConfirmationController : BaseController
    {
        private readonly ISubmissionService _submissionService;

        public ConfirmationController(ISubmissionService submissionService, ILogger logger)
            : base(logger)
        {
            _submissionService = submissionService;
        }

        [HttpGet]
        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            var data = await _submissionService.GetIlrConfirmation(Ukprn, jobId);
            return View(data);
        }
    }
}