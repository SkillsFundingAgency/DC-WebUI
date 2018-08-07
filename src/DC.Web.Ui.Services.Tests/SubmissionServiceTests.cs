using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Jobs.Model;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class SubmissionServiceTests
    {
        [Fact]
        public async Task AddMessageToQueue_Success()
        {
            var cloudStorageSettings = new CloudStorageSettings()
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxxxxxxx;AccountKey=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==;EndpointSuffix=core.windows.net",
                ContainerName = "test"
            };

            var queue = new Mock<IJobQueueService>();

            var submisisionService = new SubmissionService(queue.Object, cloudStorageSettings, null, null, null);
            await submisisionService.SubmitIlrJob(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<int>());

            queue.Verify(x => x.AddJobAsync(It.IsAny<IlrJob>()), Times.Once);
        }
    }
}