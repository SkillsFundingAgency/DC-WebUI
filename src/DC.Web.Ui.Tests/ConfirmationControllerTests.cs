using DC.Web.Ui.Controllers;
using DC.Web.Ui.Models;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DC.Web.Ui.Tests
{
    public class ConfirmationControllerTests
    {
        [Fact]
        public void ConfirmationControllerTests_Index_ValidData()
        {
            var controller = new ConfirmationController();

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["ilrSubmission"] = JsonConvert.SerializeObject(new IlrFileViewModel());
            controller.TempData = tempData;

            var result = controller.Index();

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = ((ViewResult)result).Model;
            Assert.IsAssignableFrom<IlrFileViewModel>(modelresult);
        }

        [Fact]
        public void ConfirmationControllerTests_Index_InValidData()
        {
            var controller = new ConfirmationController();
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var result = controller.Index();

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = ((ViewResult)result).Model;
            modelresult.Should().BeNull();
        }
    }
}