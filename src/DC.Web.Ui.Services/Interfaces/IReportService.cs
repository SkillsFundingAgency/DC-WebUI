using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IReportService
    {
        Task<Stream> GetReportStreamAsync(string fileName);

        Task<decimal> GetReportFileSizeAsync(FileUploadJob job);

        Task<decimal> GetReportFileSizeAsync(string fileName);

        string GetReportsZipFileName(long ukprn, long jobId, JobStatusType? crossLoadingStatus);
    }
}
