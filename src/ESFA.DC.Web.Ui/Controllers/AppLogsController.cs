using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class AppLogsController : BaseController
    {
        private readonly IAppLogsReader _appLogsReader;
        private readonly ISubmissionService _submissionService;

        public AppLogsController(IAppLogsReader appLogsReader, ISubmissionService submissionService, ILogger logger)
            : base(logger)
        {
            _appLogsReader = appLogsReader;
            _submissionService = submissionService;
        }

        public async Task<ViewResult> Index(long jobId)
        {
            try
            {
                var job = await _submissionService.GetJob(Ukprn, jobId);
                ViewBag.JobStatus = job.Status;

                return View(_appLogsReader.GetApplicationLogs(jobId));
            }
            catch (Exception e)
            {
                Logger.LogError($"Error trying to get app logs errors ukprn : {jobId}", e);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}