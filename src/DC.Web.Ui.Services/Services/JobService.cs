using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autofac.Features.Indexed;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CollectionsManagement.Models;
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
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IStorageService _storageService;

        public JobService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger,
            ICollectionManagementService collectionManagementService,
            IStorageService storageService)
        {
            _httpClient = httpClient;
            _apiBaseUrl = $"{apiSettings?.JobManagementApiBaseUrl}/job";
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _collectionManagementService = collectionManagementService;
            _storageService = storageService;
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

        public async Task<IEnumerable<SubmissonHistoryViewModel>> GetAllJobsForHistory(long ukprn, string collectionName, DateTime currentPeriodStartDateTimeUtc)
        {
            var previousPeriod = await _collectionManagementService.GetPreviousPeriodAsync(collectionName, currentPeriodStartDateTimeUtc);

            string startDatetTimeString;
            if (previousPeriod == null)
            {
                startDatetTimeString = currentPeriodStartDateTimeUtc.AddDays(-30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            }
            else
            {
                startDatetTimeString = previousPeriod.StartDateTimeUtc.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            }

            var endDatetTimeString = _dateTimeProvider.GetNowUtc().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var url = $"{_apiBaseUrl}/{ukprn}/{startDatetTimeString}/{endDatetTimeString}";
            _logger.LogInfo($"getting history url : {url}");

            var data = await _httpClient.GetDataAsync(url);
            var submissionData = _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);

            return ConvertSubmissions(submissionData);
        }

        public async Task<IEnumerable<ReportHistoryViewModel>> GetReportsHistory(long ukprn)
        {
            var startDatetTimeString = _dateTimeProvider.GetNowUtc().AddMonths(-18).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var endDatetTimeString = _dateTimeProvider.GetNowUtc().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var url = $"{_apiBaseUrl}/{ukprn}/{startDatetTimeString}/{endDatetTimeString}/latest-for-period";
            _logger.LogInfo($"getting reports history for url : {url}");

            var data = await _httpClient.GetDataAsync(url);
            var jobsList = _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);

            var result = new List<ReportHistoryViewModel>();
            foreach (var job in jobsList)
            {
                var item = result.SingleOrDefault(x => x.PeriodNumber == job.PeriodNumber && x.CollectionYear == job.CollectionYear);

                if (item == null)
                {
                    item = new ReportHistoryViewModel()
                    {
                        PeriodNumber = job.PeriodNumber,
                        CollectionYear = job.CollectionYear,
                        DisplayCollectionYear = $"20{job.CollectionYear.ToString().Substring(0, 2)} to 20{job.CollectionYear.ToString().Substring(2)}"
                    };

                    result.Add(item);
                }

                var reportSize = await _storageService.GetReportFileSizeAsync(job);
                item.CombinedFileSize += reportSize / 1024;
                item.RelatedJobs.Add(job.JobType, job.JobId);
            }

            //calculate the overall filesize and put it on the model
            foreach (var model in result)
            {
                var fileNames = string.Join(", ", model.RelatedJobs.Select(kvp => (short)kvp.Key + "-" + kvp.Value));
                model.ReportFileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileNames));
            }

            return result.OrderByDescending(x => x.CollectionYear);
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

        public async Task<FileUploadJob> GetLatestJob(long ukprn, string contractReference, string collectionName)
        {
            var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}/{contractReference}/{collectionName}/latest");
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
            return ConvertToViewModel(job);
        }

        public FileUploadConfirmationViewModel ConvertToViewModel(FileUploadJob job)
        {
            if (job?.JobId == 0)
            {
                return null;
            }

            var localTime = _dateTimeProvider.ConvertUtcToUk(job.DateTimeSubmittedUtc);
            return new FileUploadConfirmationViewModel()
            {
                FileName = job.FileName.FileNameWithoutUkprn(),
                JobId = job.JobId,
                PeriodName = string.Concat("R", job.PeriodNumber.ToString("00")),
                SubmittedAtDate = localTime.ToString("dddd dd MMMM yyyy"),
                SubmittedAtDateTime = string.Concat(localTime.ToString("hh:mmtt").ToLower(), " on ", localTime.ToString("dddd dd MMMM yyyy")),
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
                    return "EAS statement updated";
            }

            return string.Empty;
        }

        private List<SubmissonHistoryViewModel> ConvertSubmissions(IEnumerable<FileUploadJob> jobsList)
        {
            var jobsViewList = new List<SubmissonHistoryViewModel>();
            jobsList.OrderByDescending(x => x.DateTimeSubmittedUtc)
                .ToList()
                .ForEach(x => jobsViewList.Add(new SubmissonHistoryViewModel()
                {
                    JobId = x.JobId,
                    FileName = x.FileName.FileNameWithoutUkprn(),
                    JobType = MapJobType(x.JobType),
                    ReportsFileName = $"{x.JobId}_Reports.zip",
                    Status = x.Status,
                    DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(x.DateTimeSubmittedUtc).ToDateTimeDisplayFormat(),
                    SubmittedBy = x.SubmittedBy,
                    DateTimeSubmittedUtc = x.DateTimeSubmittedUtc
                }));

            return jobsViewList;
        }

        private string MapJobType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.IlrSubmission:
                    return "ILR";
                case JobType.EsfSubmission:
                    return "ESF";
                case JobType.EasSubmission:
                    return "EAS";
                default:
                    throw new Exception("invalid job type");
            }
        }
    }
}
