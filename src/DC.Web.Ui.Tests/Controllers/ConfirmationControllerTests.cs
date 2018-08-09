using DC.Web.Ui.Controllers;
using DC.Web.Ui.Controllers.IlrSubmission;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class ConfirmationControllerTests
    {
        [Fact]
        public void ConfirmationControllerTests_Index_ValidData()
        {
            var controller = new ConfirmationController(new Mock<ISubmissionService>().Object, new Mock<ILogger>().Object);

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
            var controller = new ConfirmationController(new Mock<ISubmissionService>().Object, new Mock<ILogger>().Object);
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