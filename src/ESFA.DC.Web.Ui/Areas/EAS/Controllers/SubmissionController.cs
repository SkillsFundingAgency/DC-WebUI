using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.EAS.Controllers
{
    [Area(AreaNames.Eas)]
    [Route(AreaNames.Eas + "/submission")]
    public class SubmissionController : AbstractSubmissionController
    {
        private readonly IFileNameValidationService _fileNameValidationService;

        public SubmissionController(
            IJobService jobService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IIndex<JobType, IFileNameValidationService> fileNameValidationServices,
            IIndex<JobType, IStreamableKeyValuePersistenceService> storagePersistenceServices,
            IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig> storageKeyValueConfigs)
            : base(JobType.EasSubmission, jobService, logger, collectionManagementService, storagePersistenceServices, storageKeyValueConfigs)
        {
            _fileNameValidationService = fileNameValidationServices[JobType.EasSubmission];
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

            if (await GetCurrentPeriodAsync(collectionName) == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");
                return RedirectToAction("Index", "ReturnWindowClosed", new { area = AreaNames.Esf, collectionName });
            }

            var lastSubmission = await GetLastSubmission(collectionName);
            return View(lastSubmission);
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName, IFormFile file, bool confirm)
        {
            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(file?.FileName, file?.Length, Ukprn, collectionName);
            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                return View();
            }

            if (!confirm)
            {
                AddError(ErrorMessageKeys.Submission_CheckboxFieldKey, "You must agree to this statement before you can upload a file");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "Check confirmation box");
                return View();
            }

            var jobId = await SubmitJob(collectionName, file);
            return RedirectToAction("Index", "InProgress", new { area = AreaNames.Esf, jobId });
        }
    }
}