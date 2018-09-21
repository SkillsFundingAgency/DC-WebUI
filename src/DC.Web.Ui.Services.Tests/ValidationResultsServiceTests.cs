using System;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class ValidationResultsServiceTests
    {
        [Fact]
        public void ReportFileName_Success()
        {
            var service = GetService();

            var fileName = service.GetReportFileName(new DateTime(2018, 10, 10, 20, 30, 40));
            fileName.Should().Be("Validation Errors Report 20181010-203040");
        }

        [Fact]
        public void GetStorageFileName_Success()
        {
            var service = GetService();

            var fileName = service.GetStorageFileName(1000, 500, new DateTime(2018, 10, 10, 20, 30, 40));
            fileName.Should().Be("1000/500/Validation Errors Report 20181010-203040");
        }

        [Fact]
        public void GetValidationResult_NoData()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            var service = GetService(httpClientMock.Object);

            var result = service.GetValidationResult(1000, 500, new DateTime(2018, 10, 10, 20, 30, 40)).Result;
            result.Should().BeNull();
        }

        [Fact]
        public void GetValidationResult_Success()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() =>
                "{\"TotalLearners\" :\"10\", \"TotalErrors\" : \"200\", \"TotalWarnings\" : \"20\", \"TotalErrorLearners\" : \"30\", \"TotalWarningLearners\" : \"40\"}");

            var service = GetService(httpClientMock.Object);

            var result = service.GetValidationResult(1000, 500, new DateTime(2018, 10, 10, 20, 30, 40)).Result;
            result.TotalErrors.Should().Be(200);
            result.TotalErrorLearners.Should().Be(30);
            result.TotalLearners.Should().Be(10);
            result.TotalWarningLearners.Should().Be(40);
            result.TotalWarnings.Should().Be(20);
        }

        private IValidationResultsService GetService(IBespokeHttpClient httpClient = null)
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 10, 10, 20, 30, 40));
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(new DateTime(2018, 10, 10, 20, 30, 40));

            if (httpClient == null)
            {
                var httpClientMock = new Mock<IBespokeHttpClient>();
                httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => "test data");
                httpClient = httpClientMock.Object;
            }

            var service = new ValidationResultsService(
                new JsonSerializationService(),
                new Mock<IReportService>().Object,
                dateTimeProviderMock.Object,
                httpClient,
                new ApiSettings());
            return service;
        }
    }
}