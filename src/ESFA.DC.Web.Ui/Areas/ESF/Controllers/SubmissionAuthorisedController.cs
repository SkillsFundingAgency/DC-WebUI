﻿using System;
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
    [Route(AreaNames.Esf + "/submission")]
    public class SubmissionAuthorisedController : AbstractSubmissionAuthorisedController
    {
        private readonly IFileNameValidationService _fileNameValidationService;

        public SubmissionAuthorisedController(
            IJobService jobService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IIndex<EnumJobType, IFileNameValidationService> fileNameValidationServices,
            IIndex<EnumJobType, IStreamableKeyValuePersistenceService> storagePersistenceServices,
            IIndex<EnumJobType, IAzureStorageKeyValuePersistenceServiceConfig> storageKeyValueConfigs)
            : base(EnumJobType.EsfSubmission, jobService, logger, collectionManagementService, storagePersistenceServices, storageKeyValueConfigs)
        {
            _fileNameValidationService = fileNameValidationServices[EnumJobType.EsfSubmission];
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
                return RedirectToAction("Index", "ReturnWindowClosedAuthorised");
            }

            if (await GetCurrentPeriodAsync(collectionName) == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");
                return RedirectToAction("Index", "ReturnWindowClosedAuthorised", new { area = AreaNames.Esf, collectionName });
            }

            var lastSubmission = await GetLastSubmission(collectionName);
            return View(lastSubmission);
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName, IFormFile file)
        {
            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(file?.FileName.ToUpper(), file?.Length, Ukprn, collectionName);
            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                var lastSubmission = await GetLastSubmission(collectionName);
                return View(lastSubmission);
            }

            var jobId = await SubmitJob(collectionName, file);
            return RedirectToAction("Index", "InProgressAuthorised", new { area = AreaNames.Esf, jobId });
        }
    }
}