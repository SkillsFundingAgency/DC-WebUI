using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.ViewModels;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("ilr-submission")]
    public class ILRSubmissionController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly string TempDataKey = "CollectionType";
        private readonly IFileNameValidationService _fileNameValidationService;

        public ILRSubmissionController(
            ISubmissionService submissionService,
            ILogger logger,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider,
            ICollectionManagementService collectionManagementService,
            IFileNameValidationService fileNameValidationService)
            : base(logger)
        {
            _submissionService = submissionService;
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
            _collectionManagementService = collectionManagementService;
            _fileNameValidationService = fileNameValidationService;
        }

        public string CollectionName
        {
            get { return (string)TempData[TempDataKey]; }

            set { TempData[TempDataKey] = value; }
        }

        public IActionResult Index(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                Logger.LogWarning("collection type passed in as null or empty");
                throw new Exception("null or empty collection type");
            }

            CollectionName = collectionName;

            return View();
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View();
            }

            if (_fileNameValidationService.ValidateFile(file.FileName, Ukprn) != FileNameValidationResult.Valid)
            {
                ModelState.AddModelError("fileName", "Invalid file name");
                return View();
            }

            //TODO: Validate if collection is indeed available to hhe provider, or someone has hacked in the request

            var period = await _collectionManagementService.GetCurrentPeriod(CollectionName);

            if (period == null)
            {
                Logger.LogWarning($"No active period for collection : {CollectionName}");
                throw new Exception($"No active period for collection : {CollectionName}");
            }

            try
            {
                // push file to Storage
                using (var outputStream = await _submissionService.GetBlobStream(file.FileName))
                {
                    await file.CopyToAsync(outputStream);
                }

                // add to the queue
                var jobId = await _submissionService.SubmitIlrJob(
                    file.FileName,
                    file.Length,
                    User.Name(),
                    Ukprn,
                    CollectionName,
                    period.PeriodNumber);
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