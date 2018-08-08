using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
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

        private string _reportFileName => $"{Ukprn}/{ContextJobId}/{TaskKeys.ValidationErrors}.csv";

        [Route("")]
        public async Task<IActionResult> Index(long jobId)
        {
            Logger.LogInfo($"Loading validation results page for job id : {jobId}");

            SetJobId(jobId);

            var job = await _submissionService.GetJob(User.Ukprn(), jobId);
            if (job == null)
            {
                Logger.LogInfo($"Loading validation results page for job id : {jobId}, job not found");
                return View(new ValidationResultViewModel());
            }

            var valErrors = await _validationErrorsService.GetValidationErrors(Ukprn, jobId);
            Logger.LogInfo($"Got validation results for job id : {jobId}");

            var fileSize = await _reportService.GetReportFileSizeAsync($"{Ukprn}/{jobId}/{TaskKeys.ValidationErrors}.csv");
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

        [HttpPost]
        public IActionResult Submit(bool submitFile, int totalLearners)
        {
            Logger.LogInfo($"Validation results Submit to progress : {submitFile}");
            if (!submitFile)
            {
                _submissionService.UpdateJobStatus(ContextJobId, JobStatusType.Completed, totalLearners);
                Logger.LogInfo($"Validation results Updated status to Completed successfully for job id : {ContextJobId}");
                return RedirectToAction("Index", "SubmissionOptions");
            }
            else
            {
                _submissionService.UpdateJobStatus(ContextJobId, JobStatusType.Ready, totalLearners);
                Logger.LogInfo($"Validation results Updated status to Ready successfully for job id : {ContextJobId}");
                return RedirectToAction("Index", "Confirmation", new { jobId = ContextJobId });
            }
        }

        [Route("Download")]
        public async Task<FileResult> Download()
        {
            Logger.LogInfo($"Downlaod csv request for Job id : {ContextJobId}, Filename : {_reportFileName}");

            try
            {
                var csvBlobStream = await _reportService.GetReportStreamAsync($"{Ukprn}/{ContextJobId}/{TaskKeys.ValidationErrors}.csv");
                return File(csvBlobStream, "text/csv", $"{Ukprn}_{ContextJobId}_{TaskKeys.ValidationErrors}.csv");
            }
            catch (Exception e)
            {
                Logger.LogError($"Download csv failed for job id : {ContextJobId}", e);
                throw;
            }
        }
    }
}