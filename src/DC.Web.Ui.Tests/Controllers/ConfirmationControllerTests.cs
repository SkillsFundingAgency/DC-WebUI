using DC.Web.Ui.Areas.ILR.Controllers;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class ConfirmationControllerTests
    {
        [Fact]
        public void ConfirmationControllerTests_Index_ValidData()
        {
            var controller = new SubmissionConfirmationController(new Mock<ISubmissionService>().Object, new Mock<ILogger>().Object);

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var result = controller.Index(It.IsAny<long>()).Result;

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = ((ViewResult)result).Model;
        }

        [Fact]
        public void ConfirmationControllerTests_Index_InValidData()
        {
            var controller = new SubmissionConfirmationController(new Mock<ISubmissionService>().Object, new Mock<ILogger>().Object);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var result = controller.Index(It.IsAny<long>()).Result;

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = ((ViewResult)result).Model;
            modelresult.Should().BeNull();
        }
    }
}