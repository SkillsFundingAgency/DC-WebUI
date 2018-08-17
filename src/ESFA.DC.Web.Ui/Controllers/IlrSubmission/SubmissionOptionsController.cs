using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
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
                    switch (submissionType)
                    {
                        case "ILR":
                            return RedirectToAction("Index", "CollectionOptions", new { collectionType = submissionType });
                        default:
                            throw new Exception("Not supported");
                    }
                }
            }
            else
            {
                AddFieldError(ErrorMessageKeys.SubmissionOptions_OptionsFieldKey);
                AddSummaryError(_summaryErrorMessage);

                Logger.LogInfo($"Ukprn : {Ukprn}, Invalid submittion type selected for the provider : {submissionType}");
            }

            return View("Index", data);
        }
    }
}