using DC.Web.Ui.Settings.Models;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class SubmissionServiceTests
    {
        [Fact]
        public void GetBlobStream_Success()
        {
            var cloudStorageSettings = new CloudStorageSettings()
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxxxxxxx;AccountKey=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==;EndpointSuffix=core.windows.net",
                ContainerName = "test"
            };

            var submisisionService = new SubmissionService.SubmissionService(null, cloudStorageSettings);

            submisisionService.GetBlobStream("test file").Should().BeAssignableTo<Task<CloudBlobStream>>();
        }

        [Fact]
        public async Task GetBlobStream_Error_InvalidAccount()
        {
            var cloudStorageSettings = new CloudStorageSettings()
            {
                ConnectionString = "",
                ContainerName = "test"
            };

            var submisisionService = new SubmissionService.SubmissionService(null, cloudStorageSettings);
            await Assert.ThrowsAnyAsync<Exception>(() => submisisionService.GetBlobStream("test file"));
        }

        [Fact]
        public async Task GetBlobStream_Error_InvalidFileName()
        {
            var cloudStorageSettings = new CloudStorageSettings()
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxxxxxxx;AccountKey=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==;EndpointSuffix=core.windows.net",
                ContainerName = "test"
            };

            var submisisionService = new SubmissionService.SubmissionService(null, cloudStorageSettings);
            await Assert.ThrowsAnyAsync<Exception>(() => submisisionService.GetBlobStream(null));
        }


        [Fact]
        public async Task AddMessageToQueue_Success()
        {
            var cloudStorageSettings = new CloudStorageSettings()
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxxxxxxx;AccountKey=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==;EndpointSuffix=core.windows.net",
                ContainerName = "test"
            };
            var queueClient = new Mock<IQueueClient>();

            var queue = new Mock<IServiceBusQueue>();

            var submisisionService = new SubmissionService.SubmissionService(queue.Object, cloudStorageSettings);
            await submisisionService.AddMessageToQueue(It.IsAny<string>(), It.IsAny<Guid>());

            queue.Verify(x => x.SendMessagesAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }
    }
}