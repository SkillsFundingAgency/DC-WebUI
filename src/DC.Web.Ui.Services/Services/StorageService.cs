using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Services
{
    public class StorageService : IStorageService
    {
        private readonly ILogger _logger;
        private readonly IIndex<EnumJobType, IAzureStorageKeyValuePersistenceServiceConfig> _indexedCloudStorageSettings;

        public StorageService(
            ILogger logger,
            IIndex<EnumJobType, IAzureStorageKeyValuePersistenceServiceConfig> indexedCloudStorageSettings)
        {
            _logger = logger;
            _indexedCloudStorageSettings = indexedCloudStorageSettings;
        }

        public async Task<Stream> GetBlobFileStreamAsync(string fileName, EnumJobType jobType)
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
            var fileName = GetReportsZipFileName(job.Ukprn, job.JobId);
            return await GetReportFileSizeAsync(fileName, job.JobType);
        }

        public async Task<decimal> GetReportFileSizeAsync(string fileName, EnumJobType jobType)
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

        public string GetReportsZipFileName(long ukprn, long jobId)
        {
            return $"{ukprn}/{jobId}/Reports.zip";
        }

        public async Task<Stream> GetMergedReportFile(long ukprn, Dictionary<EnumJobType, long> jobsList)
        {
            var tasks = new List<Task<Stream>>();
            foreach (var job in jobsList)
            {
                var fileName = GetReportsZipFileName(ukprn, job.Value);
                tasks.Add(GetBlobFileStreamAsync(fileName, job.Key));
            }

            await Task.WhenAll(tasks);

            var writer = new MemoryStream();
            using (var outArchive = new ZipArchive(writer, ZipArchiveMode.Create, true))
            {
                foreach (var inTask in tasks)
                {
                    await WriteEntry(inTask.Result, outArchive);
                }
            }

            writer.Seek(0, SeekOrigin.Begin);
            return writer;
        }

        public CloudBlockBlob GetBlob(string fileName, EnumJobType jobType)
        {
            var cloudStorageSettings = _indexedCloudStorageSettings[jobType];
            var cloudStorageAccount = CloudStorageAccount.Parse(cloudStorageSettings.ConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(cloudStorageSettings.ContainerName);
            return cloudBlobContainer.GetBlockBlobReference(fileName);
        }

        private async Task WriteEntry(Stream inputStream, ZipArchive outArchive)
        {
            if (inputStream == null)
            {
                return;
            }

            using (var ilrReader = new ZipArchive(inputStream, ZipArchiveMode.Read, false))
            {
                foreach (var entry in ilrReader.Entries)
                {
                    ZipArchiveEntry newEntry = outArchive.CreateEntry(entry.Name);
                    using (Stream streamOut = newEntry.Open())
                    {
                        using (var streamIn = entry.Open())
                        {
                            await streamIn.CopyToAsync(streamOut);
                        }
                    }
                }
            }
        }
    }
}
