using System.Threading.Tasks;
using DC.Web.Ui.Areas.ILR.Controllers;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
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
            var controller = new SubmissionConfirmationController(
                new Mock<ICollectionManagementService>().Object,
                new Mock<IJobService>().Object,
                new Mock<ILogger>().Object);

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
            var collectionManagementServiceMock = new Mock<ICollectionManagementService>();
            collectionManagementServiceMock.Setup(x => x.GetCurrentPeriodAsync("ILR1819"))
                .ReturnsAsync(() => new ReturnPeriodViewModel(1));

            var submissionServiceMock = new Mock<IJobService>();
            submissionServiceMock.Setup(x => x.GetConfirmation(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(() => null);

            var controller = new SubmissionConfirmationController(
               collectionManagementServiceMock.Object,
               submissionServiceMock.Object,
                new Mock<ILogger>().Object);
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