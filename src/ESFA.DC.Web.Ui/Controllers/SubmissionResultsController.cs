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
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-results")]
    public class SubmissionResultsController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IStorageService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SubmissionResultsController(IJobService jobService, ILogger logger, IStorageService reportService, IDateTimeProvider dateTimeProvider)
            : base(logger)
        {
            _jobService = jobService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IActionResult> Index()
        {
            var result = new SubmissionResultViewModel()
            {
                SubmissonHistoryViewModels = await GetSubmissionHistory()
            };

            return View(result);
        }

        [Route("DownloadReport/{jobId}")]
        public async Task<FileResult> DownloadReport(long jobId)
        {
            var job = await _jobService.GetJob(Ukprn, jobId);

            var reportFileName = _reportService.GetReportsZipFileName(Ukprn, jobId, job.CrossLoadingStatus);
            Logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}");

            try
            {
                var blobStream = await _reportService.GetBlobFileStreamAsync(reportFileName, job.JobType);
                return File(blobStream, "application/zip", $"{jobId}_Reports.zip");
            }
            catch (Exception e)
            {
                Logger.LogError($"Download zip failed for job id : {jobId}", e);
                throw;
            }
        }

        [Route("DownloadFile/{jobId}")]
        public async Task<FileResult> DownloadFile(long jobId)
        {
            var job = await _jobService.GetJob(Ukprn, jobId);

            Logger.LogInfo($"Downlaod submitted file request for Job id : {jobId}");

            try
            {
                var blobStream = await _reportService.GetBlobFileStreamAsync(job.FileName, job.JobType);
                return File(blobStream, $"application/{job.FileName.FileExtension()}", $"{job.FileName.FileNameWithoutUkprn()}");
            }
            catch (Exception e)
            {
                Logger.LogError($"Download source file failed for job id : {jobId}", e);
                throw;
            }
        }

        private async Task<List<SubmissonHistoryViewModel>> GetSubmissionHistory()
        {
            var jobsList = await _jobService.GetAllJobsForHistory(Ukprn);
            var jobsViewList = new List<SubmissonHistoryViewModel>();
            jobsList.OrderByDescending(x => x.DateTimeSubmittedUtc)
                .ToList()
                .ForEach(x => jobsViewList.Add(new SubmissonHistoryViewModel()
            {
                JobId = x.JobId,
                FileName = x.FileName.FileNameWithoutUkprn(),
                JobType = MapJobType(x.JobType),
                ReportsFileName = $"{x.JobId}_Reports.zip",
                Status = x.Status,
                DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(x.DateTimeSubmittedUtc).ToDateDisplayFormat(),
                SubmittedBy = x.SubmittedBy
            }));

            return jobsViewList;
        }

        private string MapJobType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.IlrSubmission:
                    return "ILR";
                case JobType.EsfSubmission:
                    return "ESF";
                case JobType.EasSubmission:
                    return "EAS";
                default:
                    throw new Exception("invalid job type");
            }
        }
    }
}