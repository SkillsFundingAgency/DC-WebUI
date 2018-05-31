using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.JobQueue;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.JobQueueManager.Models;
using ESFA.DC.JobQueueManager.Models.Enums;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.SubmissionService
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IJobQueueService _jobQueueService;
        private readonly CloudStorageSettings _cloudStorageSettings;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISerializationService _serializationService;

        public SubmissionService(
            IJobQueueService jobQueueService,
            CloudStorageSettings cloudStorageSettings,
            IBespokeHttpClient httpClient,
            JobQueueApiSettings apiSettings,
            ISerializationService serializationService)
        {
            _jobQueueService = jobQueueService;
            _cloudStorageSettings = cloudStorageSettings;
            _httpClient = httpClient;
            _baseUrl = apiSettings?.BaseUrl;
            _serializationService = serializationService;
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

        public async Task<Job> GetJob(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}/{jobId}");
            return _serializationService.Deserialize<Job>(data);
        }

        public async Task<IEnumerable<Job>> GetAllJobs(long ukprn)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}");
            return _serializationService.Deserialize<IEnumerable<Job>>(data);
        }
    }
}
