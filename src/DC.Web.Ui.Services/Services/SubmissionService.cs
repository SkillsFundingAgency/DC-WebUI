using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
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
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IQueuePublishService<MessageCrossLoadDctToDcftDto> _queuePublishService;
        private readonly CrossLoadMessageMapper _crossLoadMessageMapper;
        private readonly IReportService _reportService;

        public SubmissionService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider,
            IQueuePublishService<MessageCrossLoadDctToDcftDto> queuePublishService,
            CrossLoadMessageMapper crossLoadMessageMapper,
            IReportService reportService)
        {
            _httpClient = httpClient;
            _apiBaseUrl = $"{apiSettings?.JobQueueBaseUrl}/job";
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
            _queuePublishService = queuePublishService;
            _crossLoadMessageMapper = crossLoadMessageMapper;
            _reportService = reportService;
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

            //Send for cross loading
            if (result > 0)
            {
                await SendMessageForCrossLoading(result, submissionMessage);
            }

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
                FileName = (job.FileName.Contains("/") ? job.FileName.Split('/')[1] : job.FileName).ToUpper(),
                JobId = jobId,
                PeriodName = string.Concat("R", job.PeriodNumber.ToString("00")),
                SubmittedAt = string.Concat(job.DateTimeSubmittedUtc.ToString("hh:mmtt").ToLower(), " on ", job.DateTimeSubmittedUtc.ToString("dddd dd MMMM yyyy")),
                SubmittedBy = job.SubmittedBy
            };
        }

        public async Task SendMessageForCrossLoading(long jobId, SubmissionMessageViewModel submissionMessage)
        {
            var job = await GetJob(submissionMessage.Ukprn, jobId);
            if (job.CrossLoadingStatus.HasValue)
            {
                var reportsFileName =
                    _reportService.GetReportsZipFileName(submissionMessage.Ukprn, jobId, job.CrossLoadingStatus);
                var message = new MessageCrossLoadDctToDcft(
                    jobId,
                    submissionMessage.Ukprn,
                    submissionMessage.Upin,
                    submissionMessage.StorageReference,
                    submissionMessage.FileName,
                    MapJobType(submissionMessage.JobType),
                    submissionMessage.SubmittedBy,
                    submissionMessage.StorageReference,
                    reportsFileName);

                await _queuePublishService.PublishAsync(_crossLoadMessageMapper.FromMessage(message));
                await _httpClient.SendAsync($"{_apiBaseUrl}/cross-loading/status/{jobId}/{JobStatusType.MovedForProcessing}");
            }
        }

        public CrossLoadJobType MapJobType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.IlrSubmission:
                    return CrossLoadJobType.ILR;
                case JobType.EsfSubmission:
                    return CrossLoadJobType.ESF;
                case JobType.EasSubmission:
                    return CrossLoadJobType.EAS;
                default:
                    throw new Exception("unknown job type");
            }
        }
    }
}
