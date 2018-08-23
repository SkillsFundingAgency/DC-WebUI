using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
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
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.Now);

            var submisisionService = new SubmissionService(queue.Object, cloudStorageSettings, null, null, null, dateTimeProviderMock.Object);
            await submisisionService.SubmitIlrJob(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<int>());

            queue.Verify(x => x.AddJobAsync(It.IsAny<IlrJob>()), Times.Once);
        }

        [Fact]
        public async Task GetIlrConfirmation_Success()
        {
            var job = new IlrJob()
            {
                JobId = 10,
                Ukprn = 100,
                DateTimeSubmittedUtc = new DateTime(2018, 08, 09, 05, 06, 0),
                SubmittedBy = "test user",
                PeriodNumber = 6,
                FileName = "test1.xml"
            };
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => string.Empty);

            var serializationServiceMock = new Mock<IJsonSerializationService>();
            serializationServiceMock.Setup(x => x.Deserialize<IlrJob>(string.Empty)).Returns(job);

            var queue = new Mock<IJobQueueService>();

            var submisisionService = new SubmissionService(queue.Object, null, httpClientMock.Object,  new ApiSettings(), serializationServiceMock.Object, new Mock<IDateTimeProvider>().Object);
            var confirmation = await submisisionService.GetIlrConfirmation(It.IsAny<long>(), It.IsAny<long>());

            confirmation.Should().NotBeNull();
            confirmation.FileName.Should().Be("test1.xml");
            confirmation.PeriodName.Should().Be("R06");
            confirmation.SubmittedBy.Should().Be("test user");
            confirmation.SubmittedAt.Should().Be("05:06 AM on Thursday 09 August 2018");
            confirmation.FileName.Should().Be("test1.xml");
        }

        [Fact]
        public async Task UpdateJobStatus_Success()
        {
            var job = new JobStatusDto()
            {
                JobId = 10,
                JobStatus = 4
            };
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.SendDataAsync(It.IsAny<string>(), job));

            var submisisionService = new SubmissionService(new Mock<IJobQueueService>().Object, null, httpClientMock.Object, new ApiSettings(), null, new Mock<IDateTimeProvider>().Object);
            var result = await submisisionService.UpdateJobStatus(10, JobStatusType.Completed);
            httpClientMock.Verify(x => x.SendDataAsync(It.IsAny<string>(), It.IsAny<JobStatusDto>()), Times.Once());
        }
    }
}