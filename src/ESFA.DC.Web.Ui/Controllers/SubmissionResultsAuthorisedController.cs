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
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-results")]
    public class SubmissionResultsAuthorisedController : BaseAuthorisedController
    {
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;

        public SubmissionResultsAuthorisedController(
            IJobService jobService,
            ILogger logger,
            IStorageService storageService)
            : base(logger)
        {
            _jobService = jobService;
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _jobService.GetSubmissionHistory(Ukprn);
            return View(result);
        }

        [Route("DownloadReport/{ukprn}/{jobId}")]
        public async Task<FileResult> DownloadReport(long ukprn, long jobId)
        {
            var job = await _jobService.GetJob(ukprn, jobId);

            if (job == null)
            {
                Logger.LogError($"Job not found for provider,  job id : {jobId}");
                throw new Exception("invalid job id");
            }

            var reportFileName = _storageService.GetReportsZipFileName(ukprn, jobId);
            Logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}");

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
                Logger.LogError($"Download zip failed for job id : {jobId}", e);
                throw;
            }
        }

        [Route("DownloadReport/{ukprn}/{period}/{fileName}")]
        public async Task<FileResult> DownloadReport(long ukprn, int period, string fileName)
        {
            Logger.LogInfo($"Downlaod zip request for Filename : {fileName}");

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
                Logger.LogError($"Download zip failed for report name : {fileName}", e);
                throw;
            }
        }

        [Route("DownloadFile/{ukprn}/{jobId}")]
        public async Task<FileResult> DownloadFile(long ukprn, long jobId)
        {
            var job = await _jobService.GetJob(ukprn, jobId);

            Logger.LogInfo($"Downlaod submitted file request for Job id : {jobId}");

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
                Logger.LogError($"Download source file failed for job id : {jobId}", e);
                throw;
            }
        }
    }
}