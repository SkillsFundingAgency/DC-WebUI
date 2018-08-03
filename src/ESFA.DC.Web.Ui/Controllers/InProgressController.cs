﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Services.ValidationErrors;
using ESFA.DC.JobStatus.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class InProgressController : BaseController
    {
        private readonly ISubmissionService _submissionService;

        public InProgressController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.AutoRefresh = true;
            var jobStatus = await _submissionService.GetJobStatus(jobId);
            if (jobStatus != JobStatusType.Waiting)
            {
                return View();
            }

            return RedirectToAction("Index", "ValidationResults", new { jobId });
        }
    }
}