﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CrossLoad;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class SubmissionServiceTests
    {
        [Fact]
        public async Task SubmitJob_Fail()
        {
            var service = GetService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitJob(null));
        }

        [Fact]
        public async Task SubmitJob_Success()
        {
            var service = GetService();

            var job = new SubmissionMessageViewModel(EnumJobType.IlrSubmission, 100)
            {
                SubmittedBy = "test user",
                FileName = "22222_test1.xml",
                CollectionName = "ILR1819",
                FileSizeBytes = 100,
                NotifyEmail = "test@test.com",
                Period = 10,
                StorageReference = "test",
                CollectionYear = 1819
            };

            var result = service.SubmitJob(job).Result;
            result.Should().Be(1);
        }

        [Fact]
        public async Task GetJob_Success()
        {
            var job = new FileUploadJob()
            {
                JobId = 10,
                JobType = EnumJobType.IlrSubmission,
                FileName = "test.xml"
            };

            var serializationServiceMock = new Mock<IJsonSerializationService>();
            serializationServiceMock.Setup(x => x.Deserialize<FileUploadJob>(string.Empty)).Returns(job);

            var submisisionService = GetService(null, serializationServiceMock.Object);
            var confirmation = await submisisionService.GetJob(It.IsAny<long>(), It.IsAny<long>());

            confirmation.Should().NotBeNull();
            confirmation.FileName.Should().Be("test.xml");
            confirmation.JobType.Should().Be(EnumJobType.IlrSubmission);
            confirmation.JobId.Should().Be(10);
        }

        [Fact]
        public async Task GetAllJobs_Success()
        {
            var jobs = new List<FileUploadJob>()
            {
                new FileUploadJob()
                {
                    JobId = 10,
                    JobType = EnumJobType.IlrSubmission,
                    FileName = "test.xml"
                }
            };

        var serializationServiceMock = new Mock<IJsonSerializationService>();
        serializationServiceMock.Setup(x => x.Deserialize<IEnumerable<FileUploadJob>>(string.Empty)).Returns(jobs);

        var submisisionService = GetService(null, serializationServiceMock.Object);
        var confirmation = await submisisionService.GetAllJobs(It.IsAny<long>());

            confirmation.Should().NotBeNull();
            confirmation.Count().Should().Be(1);
        }

        [Fact]
        public async Task GetConfirmation_Success()
        {
            var dateTime = new DateTime(2018, 10, 10, 10, 20, 30);

            var job = new FileUploadJob()
            {
                JobId = 10,
                JobType = EnumJobType.IlrSubmission,
                FileName = "test.xml",
                PeriodNumber = 1,
                SubmittedBy = "test user",
                DateTimeSubmittedUtc = dateTime
            };

            var serializationServiceMock = new Mock<IJsonSerializationService>();
            serializationServiceMock.Setup(x => x.Deserialize<FileUploadJob>(string.Empty)).Returns(job);

            var submisisionService = GetService(null, serializationServiceMock.Object);
            var confirmation = await submisisionService.GetConfirmation(It.IsAny<long>(), It.IsAny<long>());

            confirmation.Should().NotBeNull();
            confirmation.FileName.Should().Be("TEST.XML");
            confirmation.PeriodName.Should().Be("R01");
            confirmation.SubmittedBy.Should().Be("test user");
            confirmation.SubmittedAtDateTime.Should().Be("10:20am on Wednesday 10 October 2018");
        }

        [Fact]
        public async Task GetJobStatus_Success()
        {
            var serializationServiceMock = new Mock<IJsonSerializationService>();
            serializationServiceMock.Setup(x => x.Deserialize<JobStatusType>(string.Empty)).Returns(JobStatusType.Completed);

            var submisisionService = GetService(null, serializationServiceMock.Object);
            var confirmation = await submisisionService.GetJobStatus(It.IsAny<long>());

            confirmation.Should().NotBeNull();
            confirmation.Should().Be(JobStatusType.Completed);
        }

        [Fact]
        public async Task UpdateJobStatus_Success()
        {
            var job = new JobStatusDto()
            {
                JobId = 10,
                JobStatus = 4
            };
            var submisisionService = GetService();
            var result = await submisisionService.UpdateJobStatus(10, JobStatusType.Completed);
            result.Should().Be("1");
        }

        private IJobService GetService(
            IBespokeHttpClient httpClient = null,
            IJsonSerializationService serializationService = null)
        {
            var dateTimeprovider = new Mock<IDateTimeProvider>();
            dateTimeprovider.Setup(x => x.GetNowUtc()).Returns(DateTime.Now);
            dateTimeprovider.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(new DateTime(2018, 10, 10, 10, 20, 30));

            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.SendDataAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(() => "1");
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => string.Empty);

            var jsonSerialisationMock = new Mock<IJsonSerializationService>();
            jsonSerialisationMock.Setup(x => x.Deserialize<FileUploadJob>(It.IsAny<string>())).Returns(new FileUploadJob()
            {
                CrossLoadingStatus = null,
                JobType = EnumJobType.IlrSubmission,
                Ukprn = 1000,
                JobId = 10
            });
            return new JobService(
                httpClient ?? httpClientMock.Object,
                new ApiSettings(),
                serializationService ?? jsonSerialisationMock.Object,
                dateTimeprovider.Object,
                new Mock<ILogger>().Object,
                new Mock<ICollectionManagementService>().Object,
                new Mock<IStorageService>().Object);
        }
    }
}