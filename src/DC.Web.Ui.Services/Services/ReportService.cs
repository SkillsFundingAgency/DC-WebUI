using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly CloudStorageSettings _cloudStorageSettings;
        private readonly ILogger _logger;

        public ReportService(ILogger logger, CloudStorageSettings cloudStorageSettings)
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

        public async Task<long> GetReportFileSizeAsync(string fileName)
        {
            _logger.LogInfo($"Getting report file size : {fileName}");
            try
            {
                var cloudBlockBlob = GetBlob(fileName);
                if (await cloudBlockBlob.ExistsAsync())
                {
                    return (long)(cloudBlockBlob.Properties.Length / 1024);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get report size : {fileName}", ex);
                throw;
            }

            return 0;
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
