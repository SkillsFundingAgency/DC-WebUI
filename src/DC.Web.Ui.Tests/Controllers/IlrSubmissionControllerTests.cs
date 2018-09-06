using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DC.Web.Ui.Areas.ILR.Controllers;
using DC.Web.Ui.Controllers.IlrSubmission;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.IO.Interfaces;
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
        public void SubmitIlr_Success()
        {
            var submissionServiceMock = new Mock<ISubmissionService>();
            submissionServiceMock.Setup(x => x.SubmitIlrJob(new IlrSubmissionMessageViewModel()
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
            var controller = GetController(new Mock<ISubmissionService>().Object, FileNameValidationResult.EmptyFile);
            var result = controller.Index("ILR1819", null).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact]
        public void SubmitIlr_EmptyFile()
        {
            var controller = GetController(new Mock<ISubmissionService>().Object, FileNameValidationResult.EmptyFile);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(0);
            var result = controller.Index("ILR1819", mockFile.Object).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        private ESFSubmissionController GetController(ISubmissionService submissionService, FileNameValidationResult fileNameValidationResult = FileNameValidationResult.Valid)
        {
            var fileNameValidationResultViewModel = new FileNameValidationResultViewModel()
            {
                ValidationResult = fileNameValidationResult,
                SummaryError = "summary",
                FieldError = "field error"
            };
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["CollectionType"] = "ILR"
            };

            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(10));

            mockCollectionmanagementService.Setup(x => x.IsValidCollectionAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriodAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(10));

            var mockFilenameValidationService = new Mock<IFileNameValidationService>();
            mockFilenameValidationService.Setup(x => x.ValidateFileNameAsync(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long>()))
                .ReturnsAsync(() => fileNameValidationResultViewModel);

            var mockStreamableServiceMock = new Mock<IStreamableKeyValuePersistenceService>();
            mockStreamableServiceMock.Setup(x => x.SaveAsync(It.IsAny<string>(), new MemoryStream(), default(CancellationToken))).Returns(Task.CompletedTask);

            var controller = new ESFSubmissionController(
                submissionService,
                It.IsAny<ILogger>(),
                mockCollectionmanagementService.Object,
                mockFilenameValidationService.Object,
                mockStreamableServiceMock.Object);

            controller.TempData = tempData;
            return controller;
        }
    }
}
