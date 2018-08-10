using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<long> SubmitIlrJob(string fileName, decimal fileSizeBytes, string submittedBy, long ukprn, string collectionName, int period);

        Task<IlrJob> GetJob(long ukprn, long jobId);

        Task<IEnumerable<IlrJob>> GetAllJobs(long ukprn);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status, int totalLearners);

        Task<JobStatusType> GetJobStatus(long jobId);

        Task<IlrSubmissionConfirmationViewModel> GetIlrConfirmation(long ukprn, long jobId);
    }
}
