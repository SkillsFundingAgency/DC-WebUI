using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.JobStatus.Interface;
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

        // Todo: Build the correct filename, will need the submission utc date tim

        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            Logger.LogInfo($"Loading validation results page for job id : {jobId}");

            SetJobId(jobId);

            var job = await _submissionService.GetJob(Ukprn, jobId);
            if (job == null)
            {
                Logger.LogInfo($"Loading validation results page for job id : {jobId}, job not found");
                return View(new ValidationResultViewModel());
            }

            var valErrors = await _validationErrorsService.GetValidationErrors(Ukprn, jobId);
            Logger.LogInfo($"Got validation results for job id : {jobId}");

            var fileSize = await _reportService.GetReportFileSizeAsync($"{Ukprn}/{jobId}/ValidationErrors.csv");
            Logger.LogInfo($"Got report size for job id : {jobId}");

            var result = new ValidationResultViewModel
            {
                JobId = jobId,
                ReportFileSize = fileSize,
                Filename = job.FileName,
                SubmissionDateTime = job.DateTimeSubmittedUtc,
                TotalLearners = job.TotalLearners,
                UploadedBy = job.SubmittedBy,
                TotalErrors = valErrors.Count()
            };
            Logger.LogInfo($"Returning validation results for job id : {jobId}, total errors : {result.TotalErrors}");

            return View(result);
        }

        [Route("{jobId}")]
        [HttpPost]
        public IActionResult Submit(long jobId, bool submitFile, int totalLearners)
        {
            Logger.LogInfo($"Validation results Submit to progress : {submitFile}");
            if (!submitFile)
            {
                _submissionService.UpdateJobStatus(jobId, JobStatusType.Completed, totalLearners);
                Logger.LogInfo($"Validation results Updated status to Completed successfully for job id : {jobId}");
                return RedirectToAction("Index", "SubmissionOptions");
            }
            else
            {
                _submissionService.UpdateJobStatus(jobId, JobStatusType.Ready, totalLearners);
                Logger.LogInfo($"Validation results Updated status to Ready successfully for job id : {jobId}");
                return RedirectToAction("Index", "SubmissionConfirmation", new { jobId = jobId });
            }
        }

        [Route("Download/{jobId}")]
        public async Task<FileResult> Download(long jobId)
        {
            try
            {
                var csvBlobStream = await _reportService.GetReportStreamAsync($"{Ukprn}/{jobId}/ValidationErrors.csv");
                return File(csvBlobStream, "text/csv", $"{Ukprn}_{jobId}_ValidationErrors.csv");
            }
            catch (Exception e)
            {
                Logger.LogError($"Download csv failed for job id : {jobId}", e);
                throw;
            }
        }
    }
}