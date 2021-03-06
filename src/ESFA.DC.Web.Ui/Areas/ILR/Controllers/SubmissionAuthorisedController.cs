﻿using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    [Area(AreaNames.Ilr)]
    [Route(AreaNames.Ilr + "/submission")]
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
            : base(EnumJobType.IlrSubmission, jobService, logger, collectionManagementService, storagePersistenceServices, storageKeyValueConfigs)
        {
            _fileNameValidationService = fileNameValidationServices[EnumJobType.IlrSubmission];
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

            await SetupNextPeriod(collectionName);

            if (TempData.ContainsKey(TempDataConstants.ErrorMessage))
            {
                AddError(ErrorMessageKeys.ErrorSummaryKey, TempData[TempDataConstants.ErrorMessage].ToString());
                AddError(ErrorMessageKeys.Submission_FileFieldKey, TempData[TempDataConstants.ErrorMessage].ToString());
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
            await SetupNextPeriod(collectionName);

            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(file?.FileName.ToUpper(), file?.Length, Ukprn, collectionName);
            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                Logger.LogWarning($"User uploaded invalid file with name :{file?.FileName}");
                var lastSubmission = await GetLastSubmission(collectionName);
                return View(lastSubmission);
            }

            var jobId = await SubmitJob(collectionName, file);
            return RedirectToAction("Index", "InProgressAuthorised", new { area = AreaNames.Ilr, jobId });
        }

        private async Task SetupNextPeriod(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                return;
            }

            if (await GetCurrentPeriodAsync(collectionName) == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");

                var nextPeriod = await GetNextPeriodAsync(collectionName);
                ViewData[ViewDataConstants.NextReturnOpenDate] = nextPeriod?.NextOpeningDate;
            }
        }
    }
}