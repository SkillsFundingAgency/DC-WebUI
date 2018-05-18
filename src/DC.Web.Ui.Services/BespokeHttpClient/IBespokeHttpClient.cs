using System.Threading.Tasks;
using DC.Web.Ui.Services.JobQueue;
using ESFA.DC.JobQueueManager.Models;

namespace DC.Web.Ui.Services.BespokeHttpClient
{
    public interface IBespokeHttpClient
    {
        Task<string> SendDataAsync(string url, object job);

        Task<string> GetDataAsync(string url);
    }
}
