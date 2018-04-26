using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.JobQueue;
using DC.Web.Ui.Settings.Models;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
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
                ConnectionString = string.Empty,
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

            var queue = new Mock<IJobQueueService>();

            var submisisionService = new SubmissionService.SubmissionService(queue.Object, cloudStorageSettings);
            await submisisionService.SubmitIlrJob(It.IsAny<string>(), It.IsAny<long>());

            queue.Verify(x => x.AddJobAsync(It.IsAny<Job>()), Times.Once);
        }
    }
}