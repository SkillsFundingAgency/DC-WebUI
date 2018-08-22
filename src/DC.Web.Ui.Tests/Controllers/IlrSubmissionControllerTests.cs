using System.Threading.Tasks;
using DC.Web.Ui.Controllers.IlrSubmission;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
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
            var mockCloudBlob = new Mock<CloudBlobStream>();
            submissionServiceMock.Setup(x => x.GetBlobStream("test file")).Returns(Task.FromResult(mockCloudBlob.Object));
            submissionServiceMock.Setup(x => x.SubmitIlrJob(
                "test file",
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult((long)1));

            var controller = GetController(submissionServiceMock.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(1024);

            var result = controller.Submit(mockFile.Object).Result;
            result.Should().BeOfType(typeof(RedirectToActionResult));
        }

        [Fact]
        public void SubmitIlr_NullFile()
        {
            var controller = GetController(new Mock<ISubmissionService>().Object);
            var result = controller.Submit(null).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact]
        public void SubmitIlr_EmptyFile()
        {
            var controller = GetController(new Mock<ISubmissionService>().Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(0);

            var result = controller.Submit(mockFile.Object).Result;
            result.Should().BeOfType(typeof(ViewResult));
        }

        private ILRSubmissionController GetController(ISubmissionService submissionService)
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["CollectionType"] = "ILR"
            };

            var mockCollectionmanagementService = new Mock<ICollectionManagementService>();
            mockCollectionmanagementService.Setup(x => x.GetCurrentPeriod(It.IsAny<string>()))
                .ReturnsAsync(() => new ReturnPeriodViewModel(10));

            var controller = new ILRSubmissionController(
                submissionService,
                It.IsAny<ILogger>(),
                mockCollectionmanagementService.Object);

            controller.TempData = tempData;
            return controller;
        }
    }
}
