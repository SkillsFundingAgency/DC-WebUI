using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ESF.Controllers
{
    [Area(AreaNames.Esf)]
    [Route(AreaNames.Esf + "/inprogress")]
    public class InProgressAuthorisedController : BaseAuthorisedController
    {
        private readonly IJobService _jobService;

        public InProgressAuthorisedController(IJobService jobService, ILogger logger)
            : base(logger)
        {
            _jobService = jobService;
        }

        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.AutoRefresh = true;
            //var jobStatus = await _submissionService.GetJobStatus(jobId);
            //if (jobStatus == JobStatusType.Ready)
            //{
            //    return View();
            //}

            return RedirectToAction("Index", "SubmissionConfirmationAuthorised", new { area= string.Empty, jobId });
        }
    }
}