﻿using System;
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
        private readonly IStorageService _reportService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICollectionManagementService _collectionManagementService;

        public SubmissionResultsAuthorisedController(
            IJobService jobService,
            ILogger logger,
            IStorageService reportService,
            IDateTimeProvider dateTimeProvider,
            ICollectionManagementService collectionManagementService)
            : base(logger)
        {
            _jobService = jobService;
            _reportService = reportService;
            _dateTimeProvider = dateTimeProvider;
            _collectionManagementService = collectionManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var collection = await _collectionManagementService.GetCollectionFromTypeAsync("ILR");

            if (collection != null)
            {
                var currentPeriod = await _collectionManagementService.GetPeriodAsync(collection.CollectionTitle, _dateTimeProvider.GetNowUtc());
                if (currentPeriod == null)
                {
                    currentPeriod = await _collectionManagementService.GetPreviousPeriodAsync(collection.CollectionTitle, _dateTimeProvider.GetNowUtc());
                }

                var submissions = (await _jobService.GetAllJobsForHistory(Ukprn, collection.CollectionTitle, currentPeriod.StartDateTimeUtc)).ToList();

                var result = new SubmissionResultViewModel()
                {
                    CurrentPeriodSubmissions = submissions.Where(x => x.DateTimeSubmittedUtc >= currentPeriod.StartDateTimeUtc).ToList(),
                    PreviousPeriodSubmissions = submissions.Where(x => x.DateTimeSubmittedUtc < currentPeriod.StartDateTimeUtc).ToList(),
                    PeriodName = currentPeriod.PeriodNumber.ToPeriodName(),
                    CollectionYearStart = $"20{collection.CollectionYear.ToString().Substring(0, 2)}",
                    CollectionYearEnd = $"20{collection.CollectionYear.ToString().Substring(2)}"
                };
                return View(result);
            }

            return View();
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
    }
}