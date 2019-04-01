using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels.HelpDesk;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.Controllers
{
    [Area(AreaNames.HelpDesk)]
    [Route(AreaNames.HelpDesk + "/provider")]
    public class ProviderDetailsController : BaseHelpDeskController
    {
        private readonly IProviderService _providerService;
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;
        private readonly IJobService _jobService;

        public ProviderDetailsController(
            IProviderService providerService,
            ILogger logger,
            IStorageService storageService,
            IJobService jobService)
        {
            _providerService = providerService;
            _logger = logger;
            _storageService = storageService;
            _jobService = jobService;
        }

        [HttpGet]
        [Route("{ukprn}/{searchTerm}")]
        public async Task<IActionResult> Index(long ukprn, string searchTerm)
        {
            var providerResult = await _providerService.GetProviderDetails(ukprn);
            providerResult.History = await _jobService.GetSubmissionHistory(ukprn);
            return View(providerResult);
        }

        [HttpPost]
        [Route("{ukprn}/{searchTerm}/FilterSubmissions")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterSubmissions(long ukprn, string[] jobTypeFilter)
        {
            var providerResult = await _providerService.GetProviderDetails(ukprn);

            //IsHelpSectionHidden = true;
            var history = await _jobService.GetSubmissionHistory(ukprn);
            history.SubmissionItems = history.SubmissionItems.Where(x => jobTypeFilter.Contains(x.JobType)).ToList();
            history.JobTypeFiltersList = jobTypeFilter.ToList();

            providerResult.History = history;

            return View("Index", providerResult);
        }

        [HttpPost]
        [Route("{ukprn}/{searchTerm}/FilterReports")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterReports(long ukprn, int[] reportsFilter)
        {
            //IsHelpSectionHidden = true;
            var providerResult = await _providerService.GetProviderDetails(ukprn);

            ViewData[ViewDataConstants.IsReportsSectionSelected] = true;

            var history = await _jobService.GetSubmissionHistory(ukprn);
            history.ReportHistoryItems = history.ReportHistoryItems.Where(x => reportsFilter.Contains(x.AcademicYear)).ToList();
            history.AcademicYearFiltersList = reportsFilter.ToList();

            providerResult.History = history;

            return View("Index", providerResult);
        }

        [Route("DownloadReport/{ukprn}/{jobId}")]
        public async Task<FileResult> DownloadReport(long ukprn, long jobId)
        {
            var job = await _jobService.GetJob(ukprn, jobId);

            if (job == null)
            {
                _logger.LogError($"Job not found for provider,  job id : {jobId}");
                throw new Exception("invalid job id");
            }

            var reportFileName = _storageService.GetReportsZipFileName(ukprn, jobId);
            _logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}");

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

            //TODO: Download reports check if they belong to ukprn or not
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(fileName);
                var decodedFileName = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                string[] splitStrings = decodedFileName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                Dictionary<JobType, long> dict = splitStrings.ToDictionary(
                    s => (JobType)short.Parse(s.Split('-')[0]),
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
            var job = await _jobService.GetJob(ukprn, jobId);

            _logger.LogInfo($"Downlaod submitted file request for Job id : {jobId}");

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
    }
}