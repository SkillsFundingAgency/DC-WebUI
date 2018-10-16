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
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-results")]
    public class SubmissionResultsController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly IReportService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SubmissionResultsController(ISubmissionService submissionService, ILogger logger, IReportService reportService, IDateTimeProvider dateTimeProvider)
            : base(logger)
        {
            _submissionService = submissionService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
        }

        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            var job = await _submissionService.GetJob(Ukprn, jobId);
            if (job == null)
            {
                Logger.LogInfo($"Loading submission results page for job id : {jobId}, job not found");
                return View(new SubmissionResultViewModel());
            }

            decimal fileSize = 0;
            if (job.Status == JobStatusType.Completed)
            {
                fileSize = await _reportService.GetReportFileSizeAsync(job);
                Logger.LogInfo($"Got report size for job id : {jobId}, filesize : {fileSize}");
            }
            else
            {
                Logger.LogInfo($"Got job status for job id : {jobId}, it is still : {job.Status}, cross loading status :{job.CrossLoadingStatus}");
            }

            var result = new SubmissionResultViewModel()
            {
                JobId = jobId,
                PeriodName = job.PeriodNumber.ToPeriodName(),
                PeriodNumber = job.PeriodNumber,
                FileSize = fileSize.ToString("N1"),
                Status = job.CrossLoadingStatus ?? job.Status,
                JobType = job.JobType,
                SubmissonHistoryViewModels = await GetSubmissionHistory(jobId)
            };

            return View(result);
        }

        [Route("Download/{jobId}")]
        public async Task<FileResult> Download(long jobId)
        {
            var job = await _submissionService.GetJob(Ukprn, jobId);

            var reportFileName = _reportService.GetReportsZipFileName(Ukprn, jobId, job.CrossLoadingStatus);
            Logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}");

            try
            {
                var blobStream = await _reportService.GetReportStreamAsync(reportFileName);
                return File(blobStream, "application/zip", $"{jobId}_Reports.zip");
            }
            catch (Exception e)
            {
                Logger.LogError($"Download zip failed for job id : {jobId}", e);
                throw;
            }
        }

        private async Task<List<SubmissonHistoryViewModel>> GetSubmissionHistory(long currentJobId)
        {
            var jobsList = await _submissionService.GetAllJobs(Ukprn);
            var jobsViewList = new List<SubmissonHistoryViewModel>();
            jobsList.Where(x => x.JobId != currentJobId).ToList().ForEach(x => jobsViewList.Add(new SubmissonHistoryViewModel()
            {
                FileName = x.FileName.Split('/')[1],
                JobType = x.JobType.ToString(),
                DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(x.DateTimeSubmittedUtc).ToDateDisplayFormat()
            }));

            return jobsViewList;
        }
    }
}