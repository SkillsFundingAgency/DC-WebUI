using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.SubmissionService
{
    public interface ISubmissionService
    {
        Task<CloudBlobStream> GetBlobStream(string fileName);
        Task AddMessageToQueue(string fileName,Guid correlationId);
    }
}
