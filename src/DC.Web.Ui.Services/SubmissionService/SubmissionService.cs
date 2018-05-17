using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.JobQueue;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.JobQueueManager.Models;
using ESFA.DC.JobQueueManager.Models.Enums;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.SubmissionService
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IJobQueueService _jobQueueService;
        private readonly CloudStorageSettings _cloudStorageSettings;

        public SubmissionService(IJobQueueService jobQueueService, CloudStorageSettings cloudStorageSettings)
        {
            _jobQueueService = jobQueueService;
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

        public async Task<long> SubmitIlrJob(string fileName, long ukprn)
        {
            var job = new Job()
            {
                Ukprn = ukprn,
                DateTimeSubmittedUtc = DateTime.UtcNow,
                FileName = fileName,
                StorageReference = _cloudStorageSettings.ContainerName,
                Priority = 1,
                Status = JobStatus.Ready,
                JobType = JobType.IlrSubmission
            };

            return await _jobQueueService.AddJobAsync(job);
        }
    }
}
