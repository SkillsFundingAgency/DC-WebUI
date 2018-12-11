using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IJobService
    {
        Task<long> SubmitJob(SubmissionMessageViewModel submissionMessage);

        Task<FileUploadJob> GetJob(long ukprn, long jobId);

        Task<IEnumerable<FileUploadJob>> GetAllJobs(long ukprn);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<JobStatusType> GetJobStatus(long jobId);

        Task<FileUploadConfirmationViewModel> GetConfirmation(long ukprn, long jobId);

        Task<IEnumerable<FileUploadJob>> GetAllJobsForPeriod(long ukprn, int period);

        Task<IEnumerable<SubmissonHistoryViewModel>> GetAllJobsForHistory(long ukprn, string collectionName, DateTime currentPeriodStartDateTimeUtc);

        Task<FileUploadJob> GetLatestJob(long ukprn, string collectionName);

        FileUploadConfirmationViewModel ConvertToViewModel(FileUploadJob job);

        Task<FileUploadJob> GetLatestJob(long ukprn, string contractReference, string collectionName);
    }
}
