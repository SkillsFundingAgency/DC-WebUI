using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IJobQueueService _jobQueueService;
        private readonly CloudStorageSettings _cloudStorageSettings;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SubmissionService(
            IJobQueueService jobQueueService,
            CloudStorageSettings cloudStorageSettings,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider)
        {
            _jobQueueService = jobQueueService;
            _cloudStorageSettings = cloudStorageSettings;
            _httpClient = httpClient;
            _baseUrl = apiSettings?.JobQueueBaseUrl;
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<long> SubmitIlrJob(
            string fileName,
            decimal fileSizeBytes,
            string submittedBy,
            long ukprn,
            string collectionName,
            int period)
        {
            var job = new IlrJob()
            {
                Ukprn = ukprn,
                DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc(),
                Priority = 1,
                Status = JobStatusType.Ready,
                SubmittedBy = submittedBy,
                FileName = fileName,
                IsFirstStage = true,
                StorageReference = _cloudStorageSettings.ContainerName,
                FileSize = fileSizeBytes,
                CollectionName = collectionName,
                PeriodNumber = period
            };
            return await _jobQueueService.AddJobAsync(job);
        }

        public async Task<IlrJob> GetJob(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}/{jobId}");
            return _serializationService.Deserialize<IlrJob>(data);
        }

        public async Task<JobStatusType> GetJobStatus(long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{jobId}/status");
            return _serializationService.Deserialize<JobStatusType>(data);
        }

        public async Task<IEnumerable<IlrJob>> GetAllJobs(long ukprn)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}");
            return _serializationService.Deserialize<IEnumerable<IlrJob>>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = (int)status
            };
            return await _httpClient.SendDataAsync($"{_baseUrl}/job/status", job);
        }

        public async Task<IlrSubmissionConfirmationViewModel> GetIlrConfirmation(long ukprn, long jobId)
        {
            var job = await GetJob(ukprn, jobId);
            return new IlrSubmissionConfirmationViewModel()
            {
                FileName = job.FileName,
                JobId = jobId,
                PeriodName = string.Concat("R", job.PeriodNumber.ToString("00")),
                SubmittedAt = string.Concat(job.DateTimeSubmittedUtc.ToString("hh:mm tt"), " on ", job.DateTimeSubmittedUtc.ToString("dddd dd MMMM yyyy")),
                SubmittedBy = job.SubmittedBy
            };
        }
    }
}
