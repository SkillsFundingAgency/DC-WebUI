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
        Task<long> SubmitIlrJob(IlrSubmissionMessageViewModel submissionMessage);

        Task<IlrJob> GetJob(long ukprn, long jobId);

        Task<IEnumerable<IlrJob>> GetAllJobs(long ukprn);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<JobStatusType> GetJobStatus(long jobId);

        Task<IlrSubmissionConfirmationViewModel> GetIlrConfirmation(long ukprn, long jobId);
    }
}
