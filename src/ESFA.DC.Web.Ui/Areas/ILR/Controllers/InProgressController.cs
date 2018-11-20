using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    [Area(AreaNames.Ilr)]
    [Route(AreaNames.Ilr + "/inprogress")]
    public class InProgressController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IValidationResultsService _validationResultsService;

        public InProgressController(
            IJobService jobService,
            ILogger logger,
            IValidationResultsService validationResultsService)
            : base(logger)
        {
            _jobService = jobService;
            _validationResultsService = validationResultsService;
        }

        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.AutoRefresh = true;

            var job = await _jobService.GetJob(Ukprn, jobId);
            if (job == null)
            {
                Logger.LogDebug($"Loading in progress page for job id : {jobId}, job not found");
                throw new Exception($"Loading in progress page for job id : {jobId}, job not found");
            }

            if (job.Status == JobStatusType.Failed || job.Status == JobStatusType.FailedRetry)
            {
                throw new Exception($"Loading in progress page for job id : {jobId}, job is in status ; {job.Status} - user will be sent to service error page");
            }

            if (job.Status != JobStatusType.Waiting)
            {
                return View();
            }

            var valResult = await _validationResultsService.GetValidationResult(Ukprn, jobId, job.JobType, job.DateTimeSubmittedUtc);
            if (valResult == null)
            {
                Logger.LogInfo($"Loading validation results page for job id : {jobId}, no data found");
                return View();
            }

            //if no error or warning then skip middle page
            if (valResult.TotalErrors == 0 && valResult.TotalWarnings == 0)
            {
                await _jobService.UpdateJobStatus(job.JobId, JobStatusType.Ready);
                Logger.LogInfo($"Validation results Updated status to Ready successfully for job id : {jobId}");
                return RedirectToAction("Index", "SubmissionConfirmation", new { area = string.Empty, jobId = jobId, IsCleanFile = true });
            }

            if (string.IsNullOrEmpty(valResult.ErrorMessage))
            {
                return RedirectToAction("Index", "ValidationResults", new { area = AreaNames.Ilr, jobId });
            }

            TempData[TempDataConstants.ErrorMessage] = valResult.ErrorMessage;
            return RedirectToAction("Index", "Submission", new { area = AreaNames.Ilr, collectionName = job.CollectionName });
        }
    }
}