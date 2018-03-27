using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DC.Web.Ui.Tests
{
    public class IlrSubmissionControllerTests
    {
        [Fact]
        public void SubmitIlr_Success()
        {
            var submissionServiceMock = new Mock<ISubmissionService>();
            submissionServiceMock.Setup(x => x.GetBlobStream("test file")).Returns(It.IsAny<Task<CloudBlobStream>>());
            submissionServiceMock.Setup(x => x.AddMessageToQueue("test file",It.IsAny<Guid>()));

            var controller = new ILRSubmissionController(submissionServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(1024);

            var result = controller.Submit(mockFile.Object).Result;
            result.Should().BeOfType(typeof(RedirectToActionResult));

            controller.TempData.ContainsKey("ilrSubmission").Should().BeTrue();
            //controller.TempData["ilrSubmission"].Should().BeAssignableTo<IlrFileViewModel>();
            var ilrFile = JsonConvert.DeserializeObject<IlrFileViewModel>(controller.TempData["ilrSubmission"].ToString());
            ilrFile.Should().BeAssignableTo<IlrFileViewModel>();

            ilrFile.Filename.Should().Be("test file");
            ilrFile.CorrelationId.Should().NotBeEmpty();
            ilrFile.SubmissionDateTime.Should().BeBefore(DateTime.Now);
            ilrFile.FileSize.Should().Be(1);

        }

        [Fact]
        public void SubmitIlr_NullFile()
        {
            var submissionServiceMock = new Mock<ISubmissionService>();
            var controller = new ILRSubmissionController(submissionServiceMock.Object);


            var result = controller.Submit(null).Result;
            result.Should().BeOfType(typeof(ViewResult));

        }

        [Fact]
        public void SubmitIlr_EmptyFile()
        {
            var submissionServiceMock = new Mock<ISubmissionService>();
            var controller = new ILRSubmissionController(submissionServiceMock.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(x => x.FileName).Returns("test file");
            mockFile.SetupGet(x => x.Length).Returns(0);

            var result = controller.Submit(mockFile.Object).Result;
            result.Should().BeOfType(typeof(ViewResult));

        }
    }
}
