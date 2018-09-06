using System;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    [Area("ilr")]
    [Route("ilr/submission")]
    public class ILRSubmissionController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IFileNameValidationService _fileNameValidationService;
        private readonly IStreamableKeyValuePersistenceService _storageService;

        public ILRSubmissionController(
            ISubmissionService submissionService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IFileNameValidationService fileNameValidationService,
            IStreamableKeyValuePersistenceService storageService)
            : base(logger)
        {
            _submissionService = submissionService;
            _collectionManagementService = collectionManagementService;
            _fileNameValidationService = fileNameValidationService;
            _storageService = storageService;
        }

        [HttpGet]
        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                Logger.LogWarning("collection type passed in as null or empty");
                throw new Exception("null or empty collection type");
            }

            if (!(await IsValidCollection(collectionName)))
            {
                Logger.LogWarning($"collection {collectionName} for ukprn : {Ukprn} is not open/available");
                return RedirectToAction("Index", "ReturnWindowClosed");
            }

            if (await _collectionManagementService.GetCurrentPeriodAsync(collectionName) == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");
                return RedirectToAction("Index", "ReturnWindowClosed");
            }

            return View();
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName, IFormFile file)
        {
            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(file?.FileName, file?.Length, Ukprn);
            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                AddError(ErrorMessageKeys.IlrSubmission_FileFieldKey, validationResult.FieldError);
                AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                return View();
            }

            if (!(await IsValidCollection(collectionName)))
            {
                Logger.LogWarning($"collection {collectionName} for ukprn : {Ukprn} is not open/available, but file is being uploaded");
                throw new ArgumentOutOfRangeException(collectionName);
            }

            var period = await _collectionManagementService.GetCurrentPeriodAsync(collectionName);

            if (period == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");
                throw new Exception($"No active period for collection : {collectionName}");
            }

            var fileName = $"{Ukprn}/{file.FileName}";
            try
            {
                // push file to Storage
                await _storageService.SaveAsync(fileName, file?.OpenReadStream());

                // add to the queue
                var jobId = await _submissionService.SubmitIlrJob(new IlrSubmissionMessageViewModel()
                {
                   FileName = fileName,
                   FileSizeBytes = file.Length,
                   SubmittedBy = User.Name(),
                   Ukprn = Ukprn,
                   CollectionName = collectionName,
                   Period = period.PeriodNumber,
                    NotifyEmail = User.Email()
                });
                return RedirectToAction("Index", "InProgress", new { area = "ilr", jobId });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error trying to subnmit ILR file with name : {fileName}", ex);
                throw;
            }
        }

        public async Task<bool> IsValidCollection(string collectionName)
        {
            return await _collectionManagementService.IsValidCollectionAsync(Ukprn, collectionName);
        }
    }
}