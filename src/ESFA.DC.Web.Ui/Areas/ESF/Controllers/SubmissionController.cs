using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ESF.Controllers
{
    [Area(AreaNames.Esf)]
    [Route("esf/submission")]
    public class SubmissionController : AbstractSubmissionController
    {
        private readonly IFileNameValidationService _fileNameValidationService;

        public SubmissionController(
            ISubmissionService submissionService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IIndex<JobType, IFileNameValidationService> fileNameValidationServices,
            IIndex<JobType, IStreamableKeyValuePersistenceService> storagePersistenceServices,
            IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig> storageKeyValueConfigs)
            : base(JobType.EsfSubmission, submissionService, logger, collectionManagementService, storagePersistenceServices, storageKeyValueConfigs)
        {
            _fileNameValidationService = fileNameValidationServices[JobType.EsfSubmission];
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
                AddError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                return View();
            }

            var jobId = await SubmitJob(collectionName, file);
            return RedirectToAction("Index", "InProgress", new { area = AreaNames.Esf, jobId });
        }
    }
}