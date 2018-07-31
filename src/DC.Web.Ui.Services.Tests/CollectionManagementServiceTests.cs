using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Polly;
using Polly.Registry;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class CollectionManagementServiceTests
    {
        [Fact]
        public void GetSubmissionOptions_Success()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            var serialisationService = new JsonSerializationService();
            var items = new List<CollectionType>()
            {
                new CollectionType()
                {
                    Type = "ILR",
                    Description = "ILR data submission"
                },
                new CollectionType()
                {
                    Type = "ESF",
                    Description = "ESF data submission"
                }
            };

            httpClientMock.Setup(x => x.GetDataAsync("testurl/api/org/10000")).ReturnsAsync(() => serialisationService.Serialize(items));

            var pollyRegistryMock = new Mock<IReadOnlyPolicyRegistry<string>>();
            pollyRegistryMock.Setup(x => x.Get<IAsyncPolicy>("HttpRetryPolicy")).Returns(Policy.NoOpAsync);
            var apiSettings = new ApiSettings()
            {
                CollectionManagementBaseUrl = "testurl/api"
            };

            var service = new CollectionManagementService(httpClientMock.Object, apiSettings, new JsonSerializationService());
            var data = service.GetSubmssionOptions(10000).Result;
            data.Count().Should().Be(2);
            data.Any(x => x.Name == "ILR" && x.Title == "ILR data submission").Should().BeTrue();
            data.Any(x => x.Name == "ESF" && x.Title == "ESF data submission").Should().BeTrue();
        }

        [Fact]
        public void GetSubmissionOptions_NothingFound()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync("testurl/api/org/10000")).ReturnsAsync(() => null);

            var pollyRegistryMock = new Mock<IReadOnlyPolicyRegistry<string>>();
            pollyRegistryMock.Setup(x => x.Get<IAsyncPolicy>("HttpRetryPolicy")).Returns(Policy.NoOpAsync);
            var apiSettings = new ApiSettings()
            {
                CollectionManagementBaseUrl = "testurl/api"
            };

            var service = new CollectionManagementService(httpClientMock.Object, apiSettings, new JsonSerializationService());
            var data = service.GetSubmssionOptions(10000).Result;
            data.Count().Should().Be(0);
        }
    }
}
