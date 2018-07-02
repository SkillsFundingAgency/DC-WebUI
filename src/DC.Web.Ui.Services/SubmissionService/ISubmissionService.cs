using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.SubmissionService
{
    public interface ISubmissionService
    {
        Task<CloudBlobStream> GetBlobStream(string fileName);

        Task<long> SubmitIlrJob(string fileName, decimal fileSizeBytes, string submittedBy, long ukprn);

        Task<IlrJob> GetJob(long ukprn, long jobId);

        Task<IEnumerable<IlrJob>> GetAllJobs(long ukprn);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status, int totalLearners);
    }
}
