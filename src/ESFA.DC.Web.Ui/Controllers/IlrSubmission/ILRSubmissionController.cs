using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("ilr-submission")]
    public class ILRSubmissionController : BaseController
    {
        private const string TempDataKey = "CollectionType";
        private readonly ISubmissionService _submissionService;
        private readonly ICollectionManagementService _collectionManagementService;

        public ILRSubmissionController(
            ISubmissionService submissionService,
            ILogger logger,
            ICollectionManagementService collectionManagementService)
            : base(logger)
        {
            _submissionService = submissionService;
            _collectionManagementService = collectionManagementService;
        }

        public string CollectionName
        {
            get
            {
                return (string)TempData[TempDataKey];
            }

            set
            {
                TempData[TempDataKey] = value;
            }
        }

        public IActionResult Index(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                Logger.LogWarning("collection type passed in as null or empty");
                throw new ArgumentNullException(nameof(collectionName), "null or empty collection type");
            }

            CollectionName = collectionName;

            return View();
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Submit(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Index(CollectionName);
            }

            // TODO: Validate if collection is indeed available to the provider, or someone has hacked in the request

            var period = await _collectionManagementService.GetCurrentPeriod(CollectionName);

            if (period == null)
            {
                Logger.LogWarning($"No active period for collection : {CollectionName}");
                throw new ArgumentOutOfRangeException($"No active period for collection : {CollectionName}");
            }

            try
            {
                // Change filename to include the ukprn to keep the root of the storage account clean
                string filename = $"{Ukprn}/{file.FileName}";

                // push file to Storage
                using (var outputStream = await _submissionService.GetBlobStream(filename))
                {
                    await file.CopyToAsync(outputStream);
                }

                // add to the queue
                var jobId = await _submissionService.SubmitIlrJob(filename, file.Length, User.Name(), Ukprn, CollectionName, period.PeriodNumber);
                return RedirectToAction("Index", "InProgress", new { jobId });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error trying to subnmit ILR file with name : {file.FileName}", ex);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}