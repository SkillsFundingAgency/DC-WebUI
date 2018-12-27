using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class HistoryViewComponent : ViewComponent
    {
        private readonly ILogger _logger;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;

        public HistoryViewComponent(ILogger logger, IJobService jobService, IStorageService storageService)
        {
            _logger = logger;
            _jobService = jobService;
            _storageService = storageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(long ukprn)
        {
            var result = await _jobService.GetSubmissionHistory(ukprn);
            return View(result);
        }
    }
}