using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DC.Web.Ui.Base
{
    public abstract class AbstractSubmissionAuthorisedController : BaseAuthorisedController
    {
        private readonly IJobService _jobService;
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IStreamableKeyValuePersistenceService _storageService;
        private readonly EnumJobType _jobType;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig _storageKeyValueConfig;

        protected AbstractSubmissionAuthorisedController(
            EnumJobType jobType,
            IJobService jobService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IIndex<EnumJobType, IStreamableKeyValuePersistenceService> storagePersistenceServices,
            IIndex<EnumJobType, IAzureStorageKeyValuePersistenceServiceConfig> storageKeyValueConfigs)
            : base(logger)
        {
            _jobService = jobService;
            _collectionManagementService = collectionManagementService;
            _storageService = storagePersistenceServices[jobType];
            _jobType = jobType;
            _storageKeyValueConfig = storageKeyValueConfigs[jobType];
        }

        protected async Task<long> SubmitJob(string collectionName, IFormFile file)
        {
            long jobId;

            var collection = await _collectionManagementService.GetCollectionAsync(Ukprn, collectionName);
            if (collection == null || !collection.IsOpen)
            {
                Logger.LogWarning($"collection {collectionName} for ukprn : {Ukprn} is not open/available, but file is being uploaded");
                throw new ArgumentOutOfRangeException(collectionName);
            }

            var period = await GetCurrentPeriodAsync(collectionName);

            if (period == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");
                period = await GetNextPeriodAsync(collectionName);
            }

            try
            {
                var fileName = $"{Ukprn}/{file.FileName}".ToUpper();

                // push file to Storage
                await _storageService.SaveAsync(fileName, file?.OpenReadStream());

                // add to the queue
                jobId = await _jobService.SubmitJob(new SubmissionMessageViewModel(_jobType, Ukprn)
                {
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    SubmittedBy = User.Name(),
                    CollectionName = collectionName,
                    Period = period.PeriodNumber,
                    NotifyEmail = User.Email(),
                    StorageReference = _storageKeyValueConfig.ContainerName,
                    CollectionYear = collection.CollectionYear
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error trying to subnmit ILR file with name : {file?.Name}", ex);
                throw;
            }

            return jobId;
        }

        protected async Task<bool> IsValidCollection(string collectionName)
        {
            return await _collectionManagementService.IsValidCollectionAsync(Ukprn, collectionName);
        }

        protected async Task<ReturnPeriodViewModel> GetCurrentPeriodAsync(string collectionName)
        {
            return await _collectionManagementService.GetCurrentPeriodAsync(collectionName);
        }

        protected async Task<ReturnPeriodViewModel> GetNextPeriodAsync(string collectionName)
        {
            return await _collectionManagementService.GetNextPeriodAsync(collectionName);
        }

        protected async Task<FileUploadConfirmationViewModel> GetLastSubmission(string collectionName)
        {
            var latestJob = await _jobService.GetLatestJob(Ukprn, collectionName);
            var jobViewModel = _jobService.ConvertToViewModel(latestJob);
            return jobViewModel;
        }
    }
}
