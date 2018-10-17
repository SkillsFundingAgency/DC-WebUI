using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Services
{
    public class StorageService : IStorageService
    {
        private readonly ILogger _logger;
        private readonly IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig> _indexedCloudStorageSettings;

        public StorageService(
            ILogger logger,
            IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig> indexedCloudStorageSettings)
        {
            _logger = logger;
            _indexedCloudStorageSettings = indexedCloudStorageSettings;
        }

        public async Task<Stream> GetBlobFileStreamAsync(string fileName, JobType jobType)
        {
            _logger.LogInfo($"Getting report : {fileName}");
            try
            {
                var cloudBlockBlob = GetBlob(fileName, jobType);
                if (await cloudBlockBlob.ExistsAsync())
                {
                    return await cloudBlockBlob.OpenReadAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to report : {fileName}", ex);
                throw;
            }

            return null;
        }

        public async Task<decimal> GetReportFileSizeAsync(FileUploadJob job)
        {
            var fileName = GetReportsZipFileName(job.Ukprn, job.JobId, job.CrossLoadingStatus);
            return await GetReportFileSizeAsync(fileName, job.JobType);
        }

        public async Task<decimal> GetReportFileSizeAsync(string fileName, JobType jobType)
        {
            _logger.LogInfo($"Getting report file size : {fileName}");
            try
            {
                var cloudBlockBlob = GetBlob(fileName, jobType);
                if (await cloudBlockBlob.ExistsAsync())
                {
                    return (decimal)cloudBlockBlob.Properties.Length / 1024;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get report size : {fileName}", ex);
                throw;
            }

            return 0;
        }

        public string GetReportsZipFileName(long ukprn, long jobId, JobStatusType? crossLoadingStatus)
        {
            return $"{ukprn}/{jobId}/Reports.zip";
        }

        public CloudBlockBlob GetBlob(string fileName, JobType jobType)
        {
            var cloudStorageSettings = _indexedCloudStorageSettings[jobType];
            var cloudStorageAccount = CloudStorageAccount.Parse(cloudStorageSettings.ConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(cloudStorageSettings.ContainerName);
            return cloudBlobContainer.GetBlockBlobReference(fileName);
        }
    }
}
