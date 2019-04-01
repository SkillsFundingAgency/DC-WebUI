using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Api.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels.HelpDesk;

namespace DC.Web.Ui.Services.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IJsonSerializationService _serializationService;
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public ProviderService(
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService,
            ICollectionManagementService collectionManagementService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _httpClient = httpClient;
            _serializationService = serializationService;
            _collectionManagementService = collectionManagementService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _apiBaseUrl = $"{apiSettings?.JobManagementApiBaseUrl}/org";
        }

        public async Task<ProviderSearchResultViewModel> GetSearchResults(string searchTerm)
        {
            var result = new ProviderSearchResultViewModel()
            {
                SearchTerm = searchTerm
            };

            try
            {
                var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/search/{searchTerm}");

                if (!string.IsNullOrEmpty(data))
                {
                    var providerItems = _serializationService.Deserialize<IEnumerable<ProviderDetail>>(data).ToList();

                    if (providerItems.Any())
                    {
                        foreach (var item in providerItems)
                        {
                            result.ProvidersList.Add(new ProviderDetailViewModel()
                            {
                                Name = item.Name,
                                Ukprn = item.Ukprn,
                                LastSubmittedBy = item.LastSubmittedBy,
                                LastSubmittedByEmail = item.LastSubmittedByEmail,
                                LastSubmittedDate = _dateTimeProvider.ConvertUtcToUk(item.LastSubmittedDateUtc)
                                    .ToDateWithDayDisplayFormat()
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
             _logger.LogError($"Error occured trying to get the provider search results fro search term : {HttpUtility.UrlEncode(searchTerm)}", e);
            }

            return result;
        }

        public async Task<ProviderCollectionsViewModel> GetProviderDetails(long ukprn)
        {
            try
            {
                var data = await _httpClient.GetDataAsync($"{_apiBaseUrl}/{ukprn}");

                if (!string.IsNullOrEmpty(data))
                {
                    var providerItem = _serializationService.Deserialize<ProviderDetail>(data);

                    if (providerItem != null)
                    {
                        var result = new ProviderCollectionsViewModel
                        {
                            Name = providerItem.Name,
                            Ukprn = providerItem.Ukprn,
                            SubmissionOptionViewModels = await _collectionManagementService.GetSubmssionOptionsAsync(ukprn)
                        };

                        if (result.SubmissionOptionViewModels.Any(x => x.Name.Equals("ESF", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            result.NumberOfContracts = await _collectionManagementService.GetNumberOfEsfContracts(ukprn);
                        }

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured trying to get the provider details for ukprn : {ukprn}", e);
            }

            return null;
        }
    }
}
