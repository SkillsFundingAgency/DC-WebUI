using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class IlrFileNameValidationServiceTests
    {
        [Theory]
        [InlineData(".zip")]
        [InlineData(".xml")]
        [InlineData(".ZIP")]
        [InlineData(".XML")]
        public void IsValidExtension_True(string extension)
        {
            var service = new IlrFileNameValidationService(new Mock<IKeyValuePersistenceService>().Object, new FeatureFlags(), new Mock<IJobService>().Object, new Mock<IDateTimeProvider>().Object, new Mock<IBespokeHttpClient>().Object, new ApiSettings());
            service.ValidateExtension($"testfile{extension}", "error").Should().BeNull();
        }

        [Theory]
        [InlineData(".zip1")]
        [InlineData(".mov")]
        [InlineData(".img")]
        [InlineData(".jpg")]
        public void IsValidExtension_False(string extension)
        {
            var service = GetService();
            service.ValidateExtension($"testfile{extension}", "error").Should().NotBeNull();
        }

        [Fact]
        public void IsValidUkprn_True()
        {
            var service = GetService();
            service.ValidateUkprn("ILR-10006341-1819-20180118-023456-02.xml", 10006341).Should().BeNull();
        }

        [Fact]
        public void IsValidUkprn_False()
        {
            var service = GetService();
            service.ValidateUkprn("ILR-10006341-1819-20180118-023456-02.xml", 99999999).Should().NotBeNull();
        }

        [Fact]
        public void IsValidRegex_True()
        {
            var service = GetService();
            service.IsValidRegex("ILR-10006341-1819-20180118-023456-02.xml").Should().BeTrue();
        }

        [Fact]
        public void IsValidRegex_False()
        {
            var service = GetService();
            service.IsValidRegex("ILR-10006341-1819-20180118023456-02.xml").Should().BeFalse();
        }

        [Fact]
        public void ValidateFileName_InvalidFileSize()
        {
            var service = GetService();
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 0, 10000, string.Empty).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.EmptyFile);
        }

        [Fact]
        public void ValidateFileName_InvalidUkprn()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).Throws<Exception>();

            var service = GetService(httpClientMock.Object);
            service.ValidateUkprn(10000116).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.InvalidUkprn);
        }

        [Fact]
        public void ValidateFileName_ValidUkprn()
        {
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => "true");

            var service = GetService(httpClientMock.Object);
            service.ValidateUkprn(10000116).Result.Should().Be(null);
        }

        [Fact]
        public void ValidateFileName_InvalidExtension()
        {
            var service = GetService();
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.x1ml", 10, 10000, string.Empty).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.InvalidFileExtension);
        }

        [Fact]
        public void ValidateFileName_EmptyfileName()
        {
            var service = GetService();
            service.ValidateFileNameAsync(null, 10, 10000, string.Empty).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.EmptyFile);
        }

        [Fact]
        public void ValidateFileName_Valid()
        {
            var service = GetService();
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 10, 10006341, string.Empty).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.Valid);
        }

        [Fact]
        public void ValidateFileName_FileAlredyExists()
        {
            var mockStorageService = new Mock<IKeyValuePersistenceService>();
            mockStorageService.Setup(x => x.ContainsAsync(It.IsAny<string>(), default(CancellationToken)))
                .ReturnsAsync(() => true);

            var service = new IlrFileNameValidationService(mockStorageService.Object, new FeatureFlags { DuplicateFileCheckEnabled = true }, new Mock<IJobService>().Object, new Mock<IDateTimeProvider>().Object, new Mock<IBespokeHttpClient>().Object, new ApiSettings());
            service.ValidateUniqueFileAsync("ILR-10006341-1819-20180118-023456-02.xml", 1000).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.FileAlreadyExists);
        }

        private IlrFileNameValidationService GetService(IBespokeHttpClient httpClient = null)
        {
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow.AddDays(30));
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(DateTime.UtcNow.AddDays(30));

            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.GetDataAsync(It.IsAny<string>())).ReturnsAsync(() => "true");

            return new IlrFileNameValidationService(
                new Mock<IKeyValuePersistenceService>().Object,
                new FeatureFlags(),
                new Mock<IJobService>().Object,
                dateTimeProvider.Object,
                httpClient ?? httpClientMock.Object,
                new ApiSettings());
        }
    }
}
