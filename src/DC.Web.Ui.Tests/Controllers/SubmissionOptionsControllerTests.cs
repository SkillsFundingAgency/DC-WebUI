using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class SubmissionOptionsControllerTests
    {
        [Fact]
        public void Index_Test()
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), null, It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            var items = new List<SubmissionOptionViewModel>()
            {
                new SubmissionOptionViewModel()
                {
                    Name = "ILR",
                    Title = "ILR data submission"
                },
                new SubmissionOptionViewModel()
                {
                    Name = "ESF",
                    Title = "ESF data submission"
                }
            };

            var service = new Mock<ICollectionManagementService>();
            service.Setup(x => x.GetSubmssionOptionsAsync(It.IsAny<long>())).ReturnsAsync(() => items);

            var controller = new SubmissionOptionsAuthorisedController(service.Object, mockLogger.Object);
            var result = controller.Index().Result;

            result.Should().BeOfType(typeof(ViewResult));
            var resultModel = (ViewResult)result;
            resultModel.Model.As<IEnumerable<SubmissionOptionViewModel>>().Count().Should().Be(2);
        }
    }
}
