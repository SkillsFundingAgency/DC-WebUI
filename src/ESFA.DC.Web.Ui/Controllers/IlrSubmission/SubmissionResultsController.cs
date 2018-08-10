using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.KeyGenerator.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("submission-results")]
    public class SubmissionResultsController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly IReportService _reportService;

        public SubmissionResultsController(ISubmissionService submissionService, ILogger logger, IReportService reportService)
            : base(logger)
        {
            _submissionService = submissionService;
            _reportService = reportService;
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

            var fileSize = await _reportService.GetReportFileSizeAsync($"{Ukprn}/{jobId}/reports.zip");
            Logger.LogInfo($"Got report size for job id : {jobId}");

            var result = new SubmissionResultViewModel()
            {
                JobId = jobId,
                PeriodName = job.PeriodNumber.ToPeriodName(),
                PeriodNumber = job.PeriodNumber,
                FileSize = fileSize
            };

            return View(result);
        }

        [Route("Download/{jobId}")]
        public async Task<FileResult> Download(long jobId)
        {
            var reportFileName = $"{Ukprn}/{jobId}/reports.zip";
            Logger.LogInfo($"Downlaod zip request for Job id : {jobId}, Filename : {reportFileName}");

            try
            {
                var csvBlobStream = await _reportService.GetReportStreamAsync(reportFileName);
                return File(csvBlobStream, "application/zip", reportFileName.Replace("/", "_"));
            }
            catch (Exception e)
            {
                Logger.LogError($"Download zip failed for job id : {jobId}", e);
                throw;
            }
        }
    }
}