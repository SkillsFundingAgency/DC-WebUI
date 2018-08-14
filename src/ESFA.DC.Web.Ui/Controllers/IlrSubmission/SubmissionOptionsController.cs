using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
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

        public SubmissionOptionsController(ICollectionManagementService collectionManagementService, ILogger logger)
            : base(logger)
        {
            _collectionManagementService = collectionManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _collectionManagementService.GetSubmssionOptions(Ukprn);

            if (data.Any())
            {
                Logger.LogInfo($"Ukprn : {User.Ukprn()}, returned {data.Count()} collection types ");
                return View(data);
            }

            Logger.LogInfo($"Ukprn : {User.Ukprn()}, returned no available collection types ");

            //TODO: check whih page to redirec the user to when there is not collection type available
            return RedirectToAction("Index", "NotAuthorized");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string submissionType)
        {
            Logger.LogInfo($"Ukprn : {User.Ukprn()}, submission option receievd {submissionType}");

            var data = await _collectionManagementService.GetSubmssionOptions(Ukprn);

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
                ViewData["IsValid"] = false;
                Logger.LogInfo($"Ukprn : {User.Ukprn()}, Invalid submittion type selected for the provider{submissionType}");
            }

            return View("Index", data);
        }
    }
}