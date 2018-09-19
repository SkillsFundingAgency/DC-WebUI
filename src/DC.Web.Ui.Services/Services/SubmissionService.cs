using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using Polly;
using Polly.Registry;

namespace DC.Web.Ui.Services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SubmissionService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider)
        {
            _httpClient = httpClient;
            _apiBaseUrl = $"{apiSettings?.JobQueueBaseUrl}/job";
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<long> SubmitJob(SubmissionMessageViewModel submissionMessage)
        {
            if (string.IsNullOrEmpty(submissionMessage?.FileName))
            {
                throw new ArgumentException("submission message should have file name");
            }

            var job = new FileUploadJob()
            {
                Ukprn = submissionMessage.Ukprn,
                DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc(),
                Priority = 1,
                Status = JobStatusType.Ready,
                SubmittedBy = submissionMessage.SubmittedBy,
                FileName = submissionMessage.FileName,
                IsFirstStage = true,
                StorageReference = submissionMessage.StorageReference,
                FileSize = submissionMessage.FileSizeBytes,
                CollectionName = submissionMessage.CollectionName,
                PeriodNumber = submissionMessage.Period,
                NotifyEmail = submissionMessage.NotifyEmail,
                JobType = submissionMessage.JobType
            };

            var response = await _httpClient.SendDataAsync($"{_apiBaseUrl}", job);
            long.TryParse(response, out var result);
            return result;
        }

        public async Task<FileUploadJob> GetJob(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}/{jobId}");
            return _serializationService.Deserialize<FileUploadJob>(data);
        }

        public async Task<JobStatusType> GetJobStatus(long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{jobId}/status");
            return _serializationService.Deserialize<JobStatusType>(data);
        }

        public async Task<IEnumerable<FileUploadJob>> GetAllJobs(long ukprn)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}");
            return _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = (int)status
            };
            return await _httpClient.SendDataAsync($"{_apiBaseUrl}/status", job);
        }

        public async Task<FileUploadConfirmationViewModel> GetConfirmation(long ukprn, long jobId)
        {
            var job = await GetJob(ukprn, jobId);
            return new FileUploadConfirmationViewModel()
            {
                FileName = job.FileName.Contains("/") ? job.FileName.Split('/')[1] : job.FileName,
                JobId = jobId,
                PeriodName = string.Concat("R", job.PeriodNumber.ToString("00")),
                SubmittedAt = string.Concat(job.DateTimeSubmittedUtc.ToString("hh:mm tt"), " on ", job.DateTimeSubmittedUtc.ToString("dddd dd MMMM yyyy")),
                SubmittedBy = job.SubmittedBy
            };
        }
    }
}
