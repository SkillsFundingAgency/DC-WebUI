using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly ISubmissionService _submissionService;

        private readonly ILogger _logger;

        public ReportsController(ISubmissionService submissionService, ICollectionManagementService collectionManagementService, ILogger logger)
        {
            _collectionManagementService = collectionManagementService;
            _submissionService = submissionService;
            _logger = logger;
        }

        [HttpGet]
        [Route("ValidationReport")]
        public async Task<IActionResult> ValidationReport(long jobId)
        {
            var job = await _submissionService.GetJob(User.Ukprn(), jobId);
            if (job == null)
            {
                _logger.LogError($"Job not found ; JobId: {jobId}");
                throw new Exception("invalid job id");
            }

            var period = _collectionManagementService.GetPeriod()

            return View();
        }
    }
}