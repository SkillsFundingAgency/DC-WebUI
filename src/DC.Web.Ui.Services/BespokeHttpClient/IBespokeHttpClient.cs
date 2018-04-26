using System.Threading.Tasks;
using DC.Web.Ui.Services.JobQueue;

namespace DC.Web.Ui.Services.BespokeHttpClient
{
    public interface IBespokeHttpClient
    {
        Task<string> SendDataAsync(string url, Job job);
    }
}
