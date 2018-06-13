using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.SubmissionService
{
    public interface ISubmissionService
    {
        Task<CloudBlobStream> GetBlobStream(string fileName);

        Task<long> SubmitIlrJob(string fileName, long ukprn);

        Task<Job> GetJob(long ukprn, long jobId);

        Task<IEnumerable<Job>> GetAllJobs(long ukprn);
    }
}
