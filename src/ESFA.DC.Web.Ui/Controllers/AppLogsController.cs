using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.SubmissionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Authorize]
    public class AppLogsController : Controller
    {
        private readonly IAppLogsReader _appLogsReader;
        private readonly ISubmissionService _submissionService;

        public AppLogsController(IAppLogsReader appLogsReader, ISubmissionService submissionService)
        {
            _appLogsReader = appLogsReader;
            _submissionService = submissionService;
        }

        public async Task<ViewResult> Index(long jobId)
        {
            var job = await _submissionService.GetJob(jobId);
            ViewBag.JobStatus = job.Status;

            return View(_appLogsReader.GetApplicationLogs(jobId));
        }
    }
}