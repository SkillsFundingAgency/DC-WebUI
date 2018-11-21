﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CrossLoad;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.CrossLoad.Message;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class JobService : IJobService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public JobService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _httpClient = httpClient;
            _apiBaseUrl = $"{apiSettings?.JobManagementApiBaseUrl}/job";
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
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
                JobType = submissionMessage.JobType,
                TermsAccepted = submissionMessage.JobType == JobType.EasSubmission ? true : (bool?)null,
                CollectionYear = submissionMessage.CollectionYear
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

        public async Task<IEnumerable<FileUploadJob>> GetAllJobsForPeriod(long ukprn, int period)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}/period/{period}");
            return _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);
        }

        public async Task<IEnumerable<FileUploadJob>> GetAllJobsForHistory(long ukprn)
        {
            var startDatetTimeString = _dateTimeProvider.GetNowUtc().AddDays(-60).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var endDatetTimeString = _dateTimeProvider.GetNowUtc().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var url = $"{_apiBaseUrl}/{ukprn}/{startDatetTimeString}/{endDatetTimeString}";
            _logger.LogInfo($"getting history url : {url}");
            var data = await _httpClient.GetDataAsync(url);
            return _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);
        }

        public async Task<FileUploadJob> GetLatestJob(long ukprn, string collectionName)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}/{collectionName}/latest");
            if (data == null)
            {
                return null;
            }

            return _serializationService.Deserialize<FileUploadJob>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new ESFA.DC.JobStatus.Dto.JobStatusDto()
            {
                JobId = jobId,
                JobStatus = (int)status
            };
            return await _httpClient.SendDataAsync($"{_apiBaseUrl}/status", job);
        }

        public async Task<FileUploadConfirmationViewModel> GetConfirmation(long ukprn, long jobId)
        {
            var job = await GetJob(ukprn, jobId);
            var localTime = _dateTimeProvider.ConvertUtcToUk(job.DateTimeSubmittedUtc);
            return new FileUploadConfirmationViewModel()
            {
                FileName = job.FileName.FileNameWithoutUkprn(),
                JobId = jobId,
                PeriodName = string.Concat("R", job.PeriodNumber.ToString("00")),
                SubmittedAt = string.Concat(localTime.ToString("hh:mmtt").ToLower(), " on ", localTime.ToString("dddd dd MMMM yyyy")),
                SubmittedBy = job.SubmittedBy,
                HeaderMessage = GetHeader(job.JobType, job.PeriodNumber),
                JobType = job.JobType,
                CollectionName = job.CollectionName
            };
        }

        public string GetHeader(JobType jobType, int period)
        {
            switch (jobType)
            {
                case JobType.IlrSubmission:
                    return string.Concat("R", period.ToString("00"), " ILR file submitted");
                case JobType.EsfSubmission:
                    return string.Concat("R", period.ToString("00"), " supplementary data file submitted");
                case JobType.EasSubmission:
                    return string.Concat("R", period.ToString("00"), " EAS data file submitted");
            }

            return string.Empty;
        }
    }
}