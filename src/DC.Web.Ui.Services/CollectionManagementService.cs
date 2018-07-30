using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services
{
    public class CollectionManagementService : ICollectionManagementService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IJsonSerializationService _serializationService;

        public CollectionManagementService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettings?.CollectionManagementBaseUrl;
            _serializationService = serializationService;
        }

        public async Task<IEnumerable<SubmissionOption>> GetSubmssionOptions(long ukprn)
        {
            var result = new List<SubmissionOption>();
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/org/{ukprn}");

            if (data != null)
            {
                var options = _serializationService.Deserialize<IEnumerable<CollectionType>>(data);
                options.ToList().ForEach(x => result.Add(
                    new SubmissionOption
                    {
                        Name = x.Type,
                        Title = x.Description
                    }));
            }

            return result;
        }

        public async Task<Models.ReturnPeriod> GetPeriod(string collectionName, DateTime dateTimeUtc)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/returns-calendar/{collectionName}/{dateTimeUtc}");
            Models.ReturnPeriod result = null;

            if (data != null)
            {
                var returnPeriod = _serializationService.Deserialize<ESFA.DC.CollectionsManagement.Models.ReturnPeriod>(data);
                result = new Models.ReturnPeriod(returnPeriod.PeriodNumber);
            }

            return result;
        }
    }
}
