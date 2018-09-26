using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
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
    public class ReportService : IReportService
    {
        private readonly IAzureStorageKeyValuePersistenceServiceConfig _cloudStorageSettings;
        private readonly ILogger _logger;

        public ReportService(ILogger logger, [KeyFilter(JobType.IlrSubmission)] IAzureStorageKeyValuePersistenceServiceConfig cloudStorageSettings)
        {
            _cloudStorageSettings = cloudStorageSettings;
            _logger = logger;
        }

        public async Task<Stream> GetReportStreamAsync(string fileName)
        {
            _logger.LogInfo($"Getting report : {fileName}");
            try
            {
                var cloudBlockBlob = GetBlob(fileName);
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
            return await GetReportFileSizeAsync(fileName);
        }

        public async Task<decimal> GetReportFileSizeAsync(string fileName)
        {
            _logger.LogInfo($"Getting report file size : {fileName}");
            try
            {
                var cloudBlockBlob = GetBlob(fileName);
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
            var fileNamePart = crossLoadingStatus.HasValue ? "_DC" : string.Empty;
            return $"{ukprn}/{jobId}/Reports{fileNamePart}.zip";
        }

        public CloudBlockBlob GetBlob(string fileName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            return cloudBlobContainer.GetBlockBlobReference(fileName);
        }
    }
}
