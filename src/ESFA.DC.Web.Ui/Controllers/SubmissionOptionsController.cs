using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Enums;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-options")]
    public class SubmissionOptionsController : BaseController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly ILogger _logger;

        public SubmissionOptionsController(ICollectionManagementService collectionManagementService, ILogger logger)
        {
            _collectionManagementService = collectionManagementService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _collectionManagementService.GetSubmssionOptions(User.Ukprn());

            if (data.Any())
            {
                _logger.LogInfo($"Ukprn : {User.Ukprn()}, returned {data.Count()} collection types ");
                return View(data);
            }

            _logger.LogInfo($"Ukprn : {User.Ukprn()}, returned no available collection types ");

            //TODO: check whih page to redirec the user to when there is not collection type available
            return RedirectToAction("Index", "NotAuthorized");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string submissionType)
        {
            _logger.LogInfo($"Ukprn : {User.Ukprn()}, submission option receievd {submissionType}");

            var data = await _collectionManagementService.GetSubmssionOptions(User.Ukprn());
            if (data.Any(x => x.Name == submissionType))
            {
                switch (submissionType)
                {
                    case "ILR":
                        return RedirectToAction("Index", "ILRSubmission");
                    default:
                        throw new Exception("Not supported");
                }
            }
            else
            {
                _logger.LogInfo($"Ukprn : {User.Ukprn()}, Invalid submittion type selected for the provider{submissionType}");
            }
        }
    }
}