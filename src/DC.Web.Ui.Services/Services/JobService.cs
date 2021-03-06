﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

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
                TermsAccepted = submissionMessage.JobType == EnumJobType.EasSubmission ? true : (bool?)null,
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

        public async Task<IEnumerable<SubmissonHistoryViewModel>> GetAllJobsForHistory(long ukprn)
        {
            var startDatetTimeString = _dateTimeProvider.GetNowUtc().AddDays(-90).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var endDatetTimeString = _dateTimeProvider.GetNowUtc().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            var url = $"{_apiBaseUrl}/{ukprn}/{startDatetTimeString}/{endDatetTimeString}";
            _logger.LogInfo($"getting history url : {url}");

            var data = await _httpClient.GetDataAsync(url);
            var submissionData = _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);

            return ConvertSubmissions(submissionData);
        }

        public async Task<IEnumerable<ReportHistoryViewModel>> GetReportsHistory(long ukprn)
        {
            var url = $"{_apiBaseUrl}/{ukprn}/reports-history";
            _logger.LogInfo($"getting reports history for url : {url}");

            var data = await _httpClient.GetDataAsync(url);
            var jobsList = _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);

            var result = new List<ReportHistoryViewModel>();
            foreach (var job in jobsList)
            {
                var item = result.SingleOrDefault(x => x.PeriodNumber == job.PeriodNumber && x.AcademicYear == job.CollectionYear);

                if (item == null)
                {
                    item = new ReportHistoryViewModel()
                    {
                        PeriodNumber = job.PeriodNumber,
                        AcademicYear = job.CollectionYear,
                        DisplayCollectionYear = $"20{job.CollectionYear.ToString().Substring(0, 2)} to 20{job.CollectionYear.ToString().Substring(2)}",
                        Ukprn = ukprn
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

            return result.OrderByDescending(x => x.AcademicYear);
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
                SubmittedAtDateTime = string.Concat(localTime.ToString("h:mmtt").ToLower(), " on ", localTime.ToString("dddd dd MMMM yyyy")),
                SubmittedBy = job.SubmittedBy,
                HeaderMessage = GetHeader(job.JobType, job.PeriodNumber),
                JobType = job.JobType,
                CollectionName = job.CollectionName,
            };
        }

        public string GetHeader(EnumJobType jobType, int period)
        {
            switch (jobType)
            {
                case EnumJobType.IlrSubmission:
                    return string.Concat("R", period.ToString("00"), " ILR file submitted");
                case EnumJobType.EsfSubmission:
                    return string.Concat("R", period.ToString("00"), " supplementary data file submitted");
                case EnumJobType.EasSubmission:
                    return "EAS statement updated";
            }

            return string.Empty;
        }

        public async Task<ProviderHistoryViewModel> GetSubmissionHistory(long ukprn)
        {
            var submissions = (await GetAllJobsForHistory(ukprn)).ToList();
            var reports = (await GetReportsHistory(ukprn)).ToList();

            var result = new ProviderHistoryViewModel()
            {
                Periods = submissions.GroupBy(x => x.PeriodNumber).Select(x => x.Key).OrderByDescending(x => x).ToList(),
                CollectionTypes = submissions.GroupBy(x => x.JobType).Select(x => x.Key).OrderByDescending(x => x).ToList(),
                SubmissionItems = submissions,
                ReportHistoryItems = reports,
                AcademicYears = reports.GroupBy(x => x.AcademicYear).OrderByDescending(x => x.Key).Select(x => x.Key).ToList()
            };

            return result;
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
                    DateTimeSubmittedUtc = x.DateTimeSubmittedUtc,
                    Ukprn = x.Ukprn,
                    PeriodNumber = x.PeriodNumber,
                    PeriodName = x.PeriodNumber.ToPeriodName(),
                    EsfPeriodName = $"ESF:{x.CalendarYear}_{x.CalendarMonth.ToString("00", NumberFormatInfo.InvariantInfo)}_Supp"
                }));

            return jobsViewList;
        }

        private string MapJobType(EnumJobType jobType)
        {
            switch (jobType)
            {
                case EnumJobType.IlrSubmission:
                    return "ILR";
                case EnumJobType.EsfSubmission:
                    return "ESF";
                case EnumJobType.EasSubmission:
                    return "EAS";
                default:
                    throw new Exception("invalid job type");
            }
        }
    }
}
