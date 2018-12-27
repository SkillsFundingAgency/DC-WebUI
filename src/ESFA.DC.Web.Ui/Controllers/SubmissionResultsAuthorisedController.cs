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
            var result = await _jobService.GetSubmissionHistory(Ukprn);
            return View(result);
        }
    }
}