using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services.Enums;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    [Area(AreaNames.Ilr)]
    [Route("ilr/validation-results")]
    public class ValidationResultsAuthorisedController : BaseAuthorisedController
    {
        private readonly IValidationResultsService _validationResultsService;
        private readonly IJobService _jobService;
        private readonly IStorageService _reportService;
        private readonly ICollectionManagementService _collectionManagementService;

        public ValidationResultsAuthorisedController(
            IValidationResultsService validationResultsService,
            IJobService jobService,
            IStorageService reportService,
            ICollectionManagementService collectionManagementService,
            ILogger logger)
            : base(logger)
        {
            _validationResultsService = validationResultsService;
            _jobService = jobService;
            _reportService = reportService;
            _collectionManagementService = collectionManagementService;
        }

        [Route("{jobId}")]
        [HttpGet]
        public async Task<IActionResult> Index(long jobId)
        {
            Logger.LogInfo($"Loading validation results page for job id : {jobId}");

            var job = await GetJob(jobId);

            var valResult = await _validationResultsService.GetValidationResult(Ukprn, jobId, job.JobType, job.DateTimeSubmittedUtc);
            if (valResult == null)
            {
                Logger.LogInfo($"Loading validation results page for job id : {jobId}, no data found");
                return View(new ValidationResultViewModel());
            }

            if (await _collectionManagementService.GetCurrentPeriodAsync(job.CollectionName) == null)
            {
                var nextPeriod = await _collectionManagementService.GetNextPeriodAsync(job.CollectionName);
                ViewData[ViewDataConstants.NextReturnOpenDate] = nextPeriod?.NextOpeningDate;
            }

            valResult.CollectionName = job.CollectionName;
            Logger.LogInfo($"Returning validation results for job id : {jobId}, total errors : {valResult.TotalErrors}");

            return View(valResult);
        }

        [HttpPost]
        [Route("{jobId}")]
        public async Task<IActionResult> SubmitAnyway(long jobId)
        {
            Logger.LogInfo($"Validation results Submit to progress for job id : {jobId} ");
            var job = await GetJob(jobId);

            await _jobService.UpdateJobStatus(job.JobId, JobStatusType.Ready);
            Logger.LogInfo($"Validation results Updated status to Ready successfully for job id : {jobId}");
            return RedirectToAction("Index", "SubmissionConfirmationAuthorised", new { area = string.Empty, jobId = jobId });
        }

        [HttpGet]
        [Route("SubmitAnother/{jobId}")]
        public async Task<IActionResult> SubmitAnother(long jobId)
        {
            var job = await GetJob(jobId);

            await _jobService.UpdateJobStatus(jobId, JobStatusType.Completed);
            Logger.LogInfo($"Validation results Updated status to Completed successfully for job id : {jobId}");

            return RedirectToAction("Index", "SubmissionAuthorised", new { area = AreaNames.Ilr, job.CollectionName });
        }

        [Route("Download/{jobId}")]
        public async Task<FileResult> Download(long jobId, ValidationResultsReportType reportType)
        {
            Logger.LogInfo($"Download csv request for Job id : {jobId} {reportType}");

            try
            {
                var job = await GetJob(jobId);
                var downloadFileName = $"{_validationResultsService.GetReportFileName(job.DateTimeSubmittedUtc, reportType)}.csv";
                var storageFileName = $"{_validationResultsService.GetStorageFileName(Ukprn, jobId, job.DateTimeSubmittedUtc, reportType)}.csv";

                var csvBlobStream = await _reportService.GetBlobFileStreamAsync(storageFileName, job.JobType);
                return File(csvBlobStream, "text/csv", downloadFileName);
            }
            catch (Exception e)
            {
                Logger.LogError($"Download csv failed for job id : {jobId} {reportType}", e);
                throw;
            }
        }

        public async Task<FileUploadJob> GetJob(long jobId)
        {
            Logger.LogInfo($"Trying to get Job for validation results report page for job id : {jobId}");

            var job = await _jobService.GetJob(Ukprn, jobId);
            if (job == null || job.Ukprn != Ukprn)
            {
                throw new Exception($"invalid job id provider for validation results page jobid: {jobId} and ukprn : {Ukprn}");
            }

            return job;
        }
    }
}