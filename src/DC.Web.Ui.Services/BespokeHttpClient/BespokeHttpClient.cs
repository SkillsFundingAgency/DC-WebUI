using System;
using System.Net.Http;
using System.Threading.Tasks;
using DC.Web.Ui.Services.JobQueue;
using ESFA.DC.JobQueueManager.Models;

namespace DC.Web.Ui.Services.BespokeHttpClient
{
    public class BespokeHttpClient : IBespokeHttpClient
    {
        private bool _disposed = false;
        private HttpClient _httpClient = new HttpClient();

        public async Task<string> SendDataAsync(string url, Job job)
        {
            var response = await _httpClient.PostAsJsonAsync(url, job);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
