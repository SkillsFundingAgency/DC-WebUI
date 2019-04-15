using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-results")]
    [Authorize]
    public class SubmissionResultsAuthorisedController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;

        public SubmissionResultsAuthorisedController(
            IJobService jobService,
            ILogger logger,
            IStorageService storageService)
        {
            _jobService = jobService;
            _logger = logger;
            _storageService = storageService;
        }

        [HttpGet]
        [Route("{ukprn?}/{searchTerm?}")]

        public async Task<IActionResult> Index(long? ukprn = null, string searchTerm = null)
        {
            if (!IsValidRequest(ukprn))
            {
                return RedirectToAction("Index", "NotAuthorized");
            }

            var result = await _jobService.GetSubmissionHistory(ukprn ?? User.Ukprn());
            return View(result);
        }

        [HttpPost]
        [Route("FilterSubmissions/{ukprn?}/{searchTerm?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterSubmissions(long? ukprn = null, string[] jobTypeFilter = null)
        {
            if (!IsValidRequest(ukprn))
            {
                return RedirectToAction("Index", "NotAuthorized");
            }

            var result = await _jobService.GetSubmissionHistory(ukprn ?? User.Ukprn());

            if (jobTypeFilter != null && jobTypeFilter.Any())
            {
                result.SubmissionItems = result.SubmissionItems.Where(x => jobTypeFilter.Contains(x.JobType)).ToList();
                result.CollectionTypeFiltersList = jobTypeFilter.ToList();
            }

            return View("Index", result);
        }

        [HttpPost]
        [Route("FilterReports/{ukprn?}/{searchTerm?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterReports(long? ukprn = null, int[] reportsFilter = null)
        {
            if (!IsValidRequest(ukprn))
            {
                return RedirectToAction("Index", "NotAuthorized");
            }

            ViewData[ViewDataConstants.IsReportsSectionSelected] = true;
            var result = await _jobService.GetSubmissionHistory(ukprn ?? User.Ukprn());

            if (reportsFilter != null && reportsFilter.Any())
            {
                result.ReportHistoryItems = result.ReportHistoryItems.Where(x => reportsFilter.Contains(x.AcademicYear))
                    .ToList();
                result.AcademicYearFiltersList = reportsFilter.ToList();
            }

            return View("Index", result);
        }

        [Route("DownloadReport/{ukprn}/{jobId}")]
        public async Task<FileResult> DownloadReport(long ukprn, long jobId)
        {
            if (!IsValidRequest(ukprn))
            {
                throw new Exception($"can not download the report for ukprn :{ukprn} , not matching user ukprn");
            }

            var job = await _jobService.GetJob(ukprn, jobId);

            if (job == null)
            {
                _logger.LogError($"Job not found for provider,  job id : {jobId}");
                throw new Exception("invalid job id");
            }

            var reportFileName = _storageService.GetReportsZipFileName(ukprn, jobId);
            _logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}", jobIdOverride: jobId);

            try
            {
                var blobStream = await _storageService.GetBlobFileStreamAsync(reportFileName, job.JobType);
                return new FileStreamResult(blobStream, "application/zip")
                {
                    FileDownloadName = $"{jobId}_Reports.zip"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download zip failed for job id : {jobId}", e);
                throw;
            }
        }

        [Route("DownloadReport/{ukprn}/{period}/{fileName}")]
        public async Task<FileResult> DownloadReport(long ukprn, int period, string fileName)
        {
            _logger.LogInfo($"Downlaod zip request for Filename : {fileName}");

            if (!IsValidRequest(ukprn))
            {
                throw new Exception($"can not download the report for ukprn :{ukprn} , not matching user ukprn");
            }

            try
            {
                var base64EncodedBytes = Convert.FromBase64String(fileName);
                var decodedFileName = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                string[] splitStrings = decodedFileName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                Dictionary<EnumJobType, long> dict = splitStrings.ToDictionary(
                    s => (EnumJobType)short.Parse(s.Split('-')[0]),
                    s => long.Parse(s.Split('-')[1]));

                var blobStream = await _storageService.GetMergedReportFile(ukprn, dict);
                return new FileStreamResult(blobStream, "application/zip")
                {
                    FileDownloadName = $"Reports_{period.ToPeriodName()}.zip"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download zip failed for report name : {fileName}", e);
                throw;
            }
        }

        [Route("DownloadFile/{ukprn}/{jobId}")]
        public async Task<FileResult> DownloadFile(long ukprn, long jobId)
        {
            if (!IsValidRequest(ukprn))
            {
                throw new Exception($"can not download the report for ukprn :{ukprn} , not matching user ukprn");
            }

            var job = await _jobService.GetJob(ukprn, jobId);

            _logger.LogInfo($"Downlaod submitted file request for Job id : {jobId}", jobIdOverride: jobId);

            try
            {
                var blobStream = await _storageService.GetBlobFileStreamAsync(job.FileName, job.JobType);
                return new FileStreamResult(blobStream, $"application/{job.FileName.FileExtension()}")
                {
                    FileDownloadName = $"{job.FileName.FileNameWithoutUkprn()}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download source file failed for job id : {jobId}", e);
                throw;
            }
        }

        private bool IsValidRequest(long? ukprn = null)
        {
            return User.IsHelpDeskUser() || !ukprn.HasValue || (ukprn.HasValue && ukprn == User.Ukprn());
        }
    }
}