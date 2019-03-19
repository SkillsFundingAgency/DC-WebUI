using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IStorageService
    {
        Task<Stream> GetBlobFileStreamAsync(string fileName, JobType jobType);

        Task<decimal> GetReportFileSizeAsync(FileUploadJob job);

        Task<decimal> GetReportFileSizeAsync(string fileName, JobType jobType);

        string GetReportsZipFileName(long ukprn, long jobId);

        Task<Stream> GetMergedReportFile(long ukprn, Dictionary<JobType, long> jobsList);
    }
}
