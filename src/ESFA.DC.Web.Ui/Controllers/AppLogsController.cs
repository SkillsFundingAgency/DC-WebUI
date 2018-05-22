﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Authorize]
    public class AppLogsController : Controller
    {
        private readonly IAppLogsReader _appLogsReader;
        private readonly ISubmissionService _submissionService;
        private readonly ILogger _logger;

        public AppLogsController(IAppLogsReader appLogsReader, ISubmissionService submissionService, ILogger logger)
        {
            _appLogsReader = appLogsReader;
            _submissionService = submissionService;
            _logger = logger;
        }

        public async Task<ViewResult> Index(long jobId)
        {
            try
            {
                var job = await _submissionService.GetJob(jobId);
                ViewBag.JobStatus = job.Status;

                return View(_appLogsReader.GetApplicationLogs(jobId));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error trying to get app logs errors ukprn : {jobId}", e);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}