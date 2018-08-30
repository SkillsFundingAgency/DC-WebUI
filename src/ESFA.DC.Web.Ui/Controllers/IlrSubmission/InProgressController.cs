﻿using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    public class InProgressController : BaseController
    {
        private readonly ISubmissionService _submissionService;

        public InProgressController(ISubmissionService submissionService, ILogger logger)
            : base(logger)
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