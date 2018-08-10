using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Services
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

        public async Task<IEnumerable<SubmissionOptionViewModel>> GetSubmssionOptions(long ukprn)
        {
            var result = new List<SubmissionOptionViewModel>();
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/org/{ukprn}");

            if (data != null)
            {
                var options = _serializationService.Deserialize<IEnumerable<CollectionType>>(data);
                options.ToList().ForEach(x => result.Add(
                    new SubmissionOptionViewModel
                    {
                        Name = x.Type,
                        Title = x.Description
                    }));
            }

            return result;
        }

        public async Task<ReturnPeriodViewModel> GetCurrentPeriod(string collectionName)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/returns-calendar/{collectionName}");
            ReturnPeriodViewModel result = null;

            if (data != null)
            {
                var returnPeriod = _serializationService.Deserialize<ReturnPeriod>(data);
                result = new ReturnPeriodViewModel(returnPeriod.PeriodNumber);
            }

            return result;
        }

        public async Task<IEnumerable<CollectionViewModel>> GetAvailableCollections(long ukprn, string collectionType)
        {
             var result = new List<CollectionViewModel>();
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/collections/{ukprn}/{collectionType}");

            if (data != null)
            {
                var options = _serializationService.Deserialize<IEnumerable<Collection>>(data);
                options.ToList().ForEach(x => result.Add(
                    new CollectionViewModel
                    {
                        IsOpen = x.IsOpen,
                        CollectionName = x.CollectionTitle
                    }));
            }

            return result;
        }

        public Task<bool> IsValidCollection(long ukprn, string collectionType)
        {
            throw new NotImplementedException();
        }
    }
}
