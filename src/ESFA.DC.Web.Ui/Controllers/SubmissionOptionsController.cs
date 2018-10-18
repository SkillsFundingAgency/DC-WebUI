using System;
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
    public class SubmissionOptionsController : BaseController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly string _summaryErrorMessage = "Check data you want to submit";

        public SubmissionOptionsController(ICollectionManagementService collectionManagementService, ILogger logger)
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

                //if there is one collection type skip this step
                if (data.Count == 1)
                {
                    return RedirectToNext(data.First().Name);
                }

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
                if (data.Any(x => x.Name == submissionType))
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
            switch (submissionType)
            {
                case "ILR":
                    return RedirectToAction("Index", "CollectionOptions", new { area = "ilr", collectionType = submissionType });
                case "ESF":
                    return RedirectToAction("Index", "Submission", new { area = "esf", collectionName = submissionType });
                default:
                    throw new Exception("Not supported");
            }
        }
    }
}