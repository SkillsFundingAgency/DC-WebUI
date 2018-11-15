using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Extensions;
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
            _baseUrl = apiSettings?.JobManagementApiBaseUrl;
            _serializationService = serializationService;
        }

        public async Task<IEnumerable<SubmissionOptionViewModel>> GetSubmssionOptionsAsync(long ukprn)
        {
            var result = new List<SubmissionOptionViewModel>();
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/org/{ukprn}");

            if (data != null)
            {
                var options = _serializationService.Deserialize<IEnumerable<CollectionType>>(data);
                foreach (var x in options)
                {
                    result.Add(new SubmissionOptionViewModel
                    {
                        Name = x.Type,
                        Title = x.Description
                    });
                }
            }

            return result;
        }

        public async Task<ReturnPeriodViewModel> GetCurrentPeriodAsync(string collectionName)
        {
            try
            {
                var data = await _httpClient.GetDataAsync($"{_baseUrl}/returns-calendar/{collectionName}/current");
                ReturnPeriodViewModel result = null;

                if (!string.IsNullOrEmpty(data))
                {
                    var returnPeriod = _serializationService.Deserialize<ReturnPeriod>(data);
                    result = new ReturnPeriodViewModel(returnPeriod.PeriodNumber);
                }

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ReturnPeriodViewModel> GetNextPeriodAsync(string collectionName)
        {
            try
            {
                var data = await _httpClient.GetDataAsync($"{_baseUrl}/returns-calendar/{collectionName}/next");
                ReturnPeriodViewModel result = null;

                if (!string.IsNullOrEmpty(data))
                {
                    var returnPeriod = _serializationService.Deserialize<ReturnPeriod>(data);
                    result = new ReturnPeriodViewModel(returnPeriod.PeriodNumber)
                    {
                        NextOpeningDate = returnPeriod.StartDateTimeUtc.ToDateDisplayFormat()
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CollectionViewModel>> GetAvailableCollectionsAsync(long ukprn, string collectionType)
        {
            var result = new List<CollectionViewModel>();
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/collections/{ukprn}/{collectionType}");

            if (data != null)
            {
                var options = _serializationService.Deserialize<IEnumerable<Collection>>(data);
                foreach (var x in options)
                {
                    result.Add(new CollectionViewModel
                    {
                        IsOpen = x.IsOpen,
                        CollectionName = x.CollectionTitle
                    });
                }
            }

            return result;
        }

        public async Task<bool> IsValidCollectionAsync(long ukprn, string collectionName)
        {
            var collection = await GetCollectionAsync(ukprn, collectionName);
            return collection != null && collection.IsOpen;
        }

        public async Task<Collection> GetCollectionAsync(long ukprn, string collectionName)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/org/{ukprn}/{collectionName}");

            if (!string.IsNullOrEmpty(data))
            {
                return _serializationService.Deserialize<Collection>(data);
            }

            return null;
        }
    }
}
