using DC.Web.Ui.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.IO;
using System.Linq;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Tests
{
    public class AppLogsControllerTests
    {
        [Fact]
        public void Index_Test()
        {
            var mockReader = new Mock<IAppLogsReader>();
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<string>())).Returns(It.IsAny<IEnumerable<AppLog>>());

            var controller = new AppLogsController(mockReader.Object);
            var result = controller.Index(It.IsAny<string>());

            result.Should().BeOfType(typeof(ViewResult));
        }


        [Fact]
        public void Index_Test_Data()
        {
            var appLogs = new List<AppLog>()
            {
                new AppLog(),
                new AppLog()
            };
            var mockReader = new Mock<IAppLogsReader>();
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<string>())).Returns(appLogs);

            var controller = new AppLogsController(mockReader.Object);
            var result = controller.Index(It.IsAny<string>());

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = ((ViewResult)result).Model;
            Assert.IsAssignableFrom<IEnumerable<AppLog>>(modelresult);
            ((IEnumerable<AppLog>) modelresult).Count().Should().Be(2);
        }

    }
}
