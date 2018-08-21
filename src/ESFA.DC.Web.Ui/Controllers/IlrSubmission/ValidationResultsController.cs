using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.KeyGenerator.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("validation-results")]
    public class ValidationResultsController : BaseController
    {
        private readonly IValidationErrorsService _validationErrorsService;
        private readonly ISubmissionService _submissionService;
        private readonly IReportService _reportService;

        public ValidationResultsController(
            IValidationErrorsService validationErrorsService,
            ISubmissionService submissionService,
            IReportService reportService,
            ILogger logger)
            : base(logger)
        {
            _validationErrorsService = validationErrorsService;
            _submissionService = submissionService;
            _reportService = reportService;
        }

        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            Logger.LogInfo($"Loading validation results page for job id : {jobId}");

            var job = await GetJob(jobId);

            var valResult = await _validationErrorsService.GetValidationResult(Ukprn, jobId, job.DateTimeSubmittedUtc);
            if (valResult == null)
            {
                Logger.LogInfo($"Loading validation results page for job id : {jobId}, no data found");
                return View(new ValidationResultViewModel());
            }

            valResult.CollectionName = job.CollectionName;
            Logger.LogInfo($"Returning validation results for job id : {jobId}, total errors : {valResult.TotalErrors}");

            return View(valResult);
        }

        [HttpPost]
        [Route("SubmitAnyway/{jobId}")]
        public async Task<IActionResult> SubmitAnyway(long jobId)
        {
            Logger.LogInfo($"Validation results Submit to progress for job id : {jobId} ");
            var job = await GetJob(jobId);

            await _submissionService.UpdateJobStatus(jobId, JobStatusType.Ready, -1);
            Logger.LogInfo($"Validation results Updated status to Ready successfully for job id : {jobId}");
            return RedirectToAction("Index", "SubmissionConfirmation", new { jobId = jobId });
        }

        [HttpGet]
        [Route("SubmitAnother/{jobId}")]
        public async Task<IActionResult> SubmitAnother(long jobId)
        {
            var job = await GetJob(jobId);

            await _submissionService.UpdateJobStatus(jobId, JobStatusType.Completed, -1);
            Logger.LogInfo($"Validation results Updated status to Completed successfully for job id : {jobId}");

            return RedirectToAction("Index", "ILRSubmission", new { job.CollectionName });
        }

        [Route("Download/{jobId}")]
        public async Task<FileResult> Download(long jobId)
        {
            Logger.LogInfo($"Downlaod csv request for Job id : {jobId}");

            try
            {
                var job = await GetJob(jobId);
                var fileName = $"{_validationErrorsService.GetFileName(Ukprn, jobId, job.DateTimeSubmittedUtc)}.csv";
                var csvBlobStream = await _reportService.GetReportStreamAsync(fileName);
                return File(csvBlobStream, "text/csv", fileName.Replace("/", "_"));
            }
            catch (Exception e)
            {
                Logger.LogError($"Download csv failed for job id : {jobId}", e);
                throw;
            }
        }

        private async Task<IlrJob> GetJob(long jobId)
        {
            Logger.LogInfo($"Trying to get Job for validation results report page for job id : {jobId}");

            var job = await _submissionService.GetJob(Ukprn, jobId);
            if (job == null)
            {
                throw new Exception($"invalid job id provider for validation results page jobid: {jobId}");
            }

            return job;
        }
    }
}