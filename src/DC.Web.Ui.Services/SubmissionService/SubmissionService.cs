using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.ServiceBus;
using DC.Web.Ui.Settings.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace DC.Web.Ui.Services.SubmissionService
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IServiceBusQueue _serviceBusQueue;
        private readonly CloudStorageSettings _cloudStorageSettings;

        public SubmissionService(IServiceBusQueue serviceBusQueue, CloudStorageSettings cloudStorageSettings)
        {
            _serviceBusQueue = serviceBusQueue;
            _cloudStorageSettings = cloudStorageSettings;
        }

        public async Task<CloudBlobStream> GetBlobStream(string fileName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.OpenWriteAsync();
        }

        public async Task AddMessageToQueue(string fileName, Guid correlationId)
        {
            var ilrFile = new IlrSubmissionMessage()
            {
                CorrelationId = correlationId,
                ContainerReference = _cloudStorageSettings.ContainerName,
                Filename = fileName,
            };
            await _serviceBusQueue.SendMessagesAsync(JsonConvert.SerializeObject(ilrFile), ilrFile.CorrelationId.ToString());
        }

    }
}
