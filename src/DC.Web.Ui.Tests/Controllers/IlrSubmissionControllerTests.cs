using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using DC.Web.Ui.Areas.ILR.Controllers;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class IlrSubmissionControllerTests
    {
        [Fact]
        public void Index_Success()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "test"
            };

            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.IsValidCollectionAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(1));

            var controller = GetController(null, FileNameValidationResult.Valid, mockCollectionmanagementService.Object);
            controller.TempData = tempData;

            var result = controller.Index("ILR1819").Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact]
        public async Task Index_EmptyCollectionName()
        {
            var controller = GetController(null);
            await Assert.ThrowsAsync<Exception>(() => controller.Index(null));
        }

        [Fact]
        public void Index_FailureInvalidCollection()
        {
            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.IsValidCollectionAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => false);

            var controller = GetController(null, FileNameValidationResult.Valid, mockCollectionmanagementService.Object);

            var result = controller.Index("ILR1819").Result;
            result.Should().BeOfType(typeof(RedirectToActionResult));
        }

        [Fact]
        public void Index_FailureInvalidPeriod()
        {
            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var controller = GetController(null, FileNameValidationResult.Valid, mockCollectionmanagementService.Object);

            var result = controller.Index("ILR1819").Result;
            result.Should().BeOfType(typeof(RedirectToActionResult));
        }

        [Fact]
        public void SubmitIlr_Success()
        {
            var submissionServiceMock = new Mock<IJobService>();
            submissionServiceMock.Setup(x => x.SubmitJob(new SubmissionMessageViewModel(JobType.IlrSubmission, 10)
            {
                FileName = "test file",
            })).Returns(Task.FromResult((long)1));

            var controller = GetController(submissionServiceMock.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(1024);

            var result = controller.Index("ILR1819", mockFile.Object).Result;
            result.Should().BeOfType(typeof(RedirectToActionResult));
        }

        [Fact]
        public void SubmitIlr_NullFile()
        {
            var controller = GetController(new Mock<IJobService>().Object, FileNameValidationResult.EmptyFile);
            var result = controller.Index("ILR1819", null).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact]
        public void SubmitIlr_EmptyFile()
        {
            var controller = GetController(new Mock<IJobService>().Object, FileNameValidationResult.EmptyFile);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(0);
            var result = controller.Index("ILR1819", mockFile.Object).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        private SubmissionController GetController(IJobService jobService, FileNameValidationResult fileNameValidationResult = FileNameValidationResult.Valid, ICollectionManagementService collectionManagementService = null)
        {
            var fileNameValidationResultViewModel = new FileNameValidationResultViewModel()
            {
                ValidationResult = fileNameValidationResult,
                SummaryError = "summary",
                FieldError = "field error"
            };

            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(10));

            mockCollectionmanagementService.Setup(x => x.IsValidCollectionAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);
            mockCollectionmanagementService.Setup(x => x.GetCollectionAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => new Collection { IsOpen = true, CollectionYear = 1819 });
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(10));

            var mockFilenameValidationService = new Mock<IFileNameValidationService>();
            mockFilenameValidationService.Setup(x => x.ValidateFileNameAsync(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => fileNameValidationResultViewModel);

            var mockStreamableServiceMock = new Mock<IStreamableKeyValuePersistenceService>();
            mockStreamableServiceMock.Setup(x => x.SaveAsync(It.IsAny<string>(), new MemoryStream(), default(CancellationToken))).Returns(Task.CompletedTask);

            var servicesMock = new Mock<IIndex<JobType, IStreamableKeyValuePersistenceService>>();
            servicesMock.Setup(x => x[JobType.IlrSubmission]).Returns(mockStreamableServiceMock.Object);

            var configs = new Mock<IIndex<JobType, IAzureStorageKeyValuePersistenceServiceConfig>>();
            configs.Setup(x => x[JobType.IlrSubmission]).Returns(new CloudStorageSettings());

            var filevalidationServicesMock = new Mock<IIndex<JobType, IFileNameValidationService>>();
            filevalidationServicesMock.Setup(x => x[JobType.IlrSubmission]).Returns(mockFilenameValidationService.Object);

            var controller = new SubmissionController(
                jobService,
                new Mock<ILogger>().Object,
                collectionManagementService ?? mockCollectionmanagementService.Object,
                filevalidationServicesMock.Object,
                servicesMock.Object,
                configs.Object);

            return controller;
        }
    }
}
