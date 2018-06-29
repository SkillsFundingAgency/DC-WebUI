using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Jobs.Model;
using Polly;
using Polly.Registry;

namespace DC.Web.Ui.Services.JobQueue
{
    public class JobQueueService : IJobQueueService
    {
        private readonly string _apiBaseUrl;
        private readonly IReadOnlyPolicyRegistry<string> _pollyRegistry;
        private readonly IBespokeHttpClient _httpClient;

        public JobQueueService(JobQueueApiSettings apiSettings, IReadOnlyPolicyRegistry<string> pollyRegistry, IBespokeHttpClient httpClient)
        {
            _apiBaseUrl = apiSettings.BaseUrl;
            _pollyRegistry = pollyRegistry;
            _httpClient = httpClient;
        }

        public async Task<long> AddJobAsync(IlrJob job)
        {
            var policy = _pollyRegistry.Get<IAsyncPolicy>("HttpRetryPolicy");
            var response = await policy.ExecuteAsync(
                () => _httpClient.SendDataAsync($"{_apiBaseUrl}/job", job));

            long.TryParse(response, out var result);
            return result;
        }
    }
}
