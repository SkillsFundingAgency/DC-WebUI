﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-options")]
    public class SubmissionOptionsAuthorisedController : BaseAuthorisedController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly string _summaryErrorMessage = "Check data you want to submit";

        public SubmissionOptionsAuthorisedController(ICollectionManagementService collectionManagementService, ILogger logger)
            : base(logger)
        {
            _collectionManagementService = collectionManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var data = (await _collectionManagementService.GetSubmssionOptionsAsync(Ukprn)).ToList();

            if (data.Any())
            {
                Logger.LogInfo($"Ukprn : {Ukprn}, returned {data.Count()} collection types ");
                return View(data);
            }

            Logger.LogInfo($"Ukprn : {User.Ukprn()}, returned no available collection types ");
            return RedirectToAction("Index", "NotAuthorized");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string submissionType)
        {
            Logger.LogInfo($"Ukprn : {Ukprn}, submission option receievd {submissionType}");

            var data = await _collectionManagementService.GetSubmssionOptionsAsync(Ukprn);

            if (!string.IsNullOrEmpty(submissionType))
            {
                if (submissionType == "Reports" || data.Any(x => x.Name == submissionType))
                {
                    return RedirectToNext(submissionType);
                }
            }
            else
            {
                AddError(ErrorMessageKeys.SubmissionOptions_OptionsFieldKey);
                AddError(ErrorMessageKeys.ErrorSummaryKey, _summaryErrorMessage);

                Logger.LogInfo($"Ukprn : {Ukprn}, Invalid submittion type selected for the provider : {submissionType}");
            }

            return View("Index", data);
        }

        private IActionResult RedirectToNext(string submissionType)
        {
            if (submissionType.Equals("Reports", StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction("Index", "SubmissionResultsAuthorised");
            }

            return RedirectToAction("Index", "CollectionOptionsAuthorised", new { collectionType = submissionType });
        }
    }
}