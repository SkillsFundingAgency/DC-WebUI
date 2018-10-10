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
    public abstract class AbstractSubmissionController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IStreamableKeyValuePersistenceService _storageService;
        private readonly JobType _jobType;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig _storageKeyValueConfig;

        protected AbstractSubmissionController(
            JobType jobType,
            ISubmissionService submissionService,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IIndex<JobType, IStreamableKeyValuePersistenceService> storagePersistenceServices,
            IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig> storageKeyValueConfigs)
            : base(logger)
        {
            _submissionService = submissionService;
            _collectionManagementService = collectionManagementService;
            _storageService = storagePersistenceServices[jobType];
            _jobType = jobType;
            _storageKeyValueConfig = storageKeyValueConfigs[jobType];
        }

        protected async Task<long> SubmitJob(string collectionName, IFormFile file)
        {
            long jobId;

            if (!(await IsValidCollection(collectionName)))
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
                var fileName = $"{Ukprn}/{file.FileName}";

                // push file to Storage
                await _storageService.SaveAsync(fileName, file?.OpenReadStream());

                // add to the queue
                jobId = await _submissionService.SubmitJob(new SubmissionMessageViewModel(_jobType, Ukprn, Upin)
                {
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    SubmittedBy = User.Name(),
                    CollectionName = collectionName,
                    Period = period.PeriodNumber,
                    NotifyEmail = User.Email(),
                    StorageReference = _storageKeyValueConfig.ContainerName
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
    }
}
