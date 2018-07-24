using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Models;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Logging.Interfaces;
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
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            var items = new List<SubmissionOption>()
            {
                new SubmissionOption()
                {
                    Name = "ILR",
                    Title = "ILR data submission"
                }
            };

            var service = new Mock<ICollectionManagementService>();
            service.Setup(x => x.GetSubmssionOptions(It.IsAny<long>())).ReturnsAsync(() => items);

            var controller = new SubmissionOptionsController(service.Object, mockLogger.Object);
            var result = controller.Index().Result;

            result.Should().BeOfType(typeof(ViewResult));
            var resultModel = (ViewResult)result;
            resultModel.Model.As<IEnumerable<SubmissionOption>>().Count().Should().Be(1);
        }
    }
}
