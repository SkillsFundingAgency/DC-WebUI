﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class AppLogsControllerTests
    {
        [Fact]
        public void Index_Test()
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            var mockReader = new Mock<IAppLogsReader>();
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<long>())).Returns(It.IsAny<IEnumerable<AppLogViewModel>>());

            var controller = new AppLogsController(mockReader.Object, null, mockLogger.Object);
            var result = controller.Index(It.IsAny<long>());

            result.Should().BeOfType(typeof(Task<ViewResult>));
        }

        [Fact]
        public void Index_Test_Data()
        {
            var appLogs = new List<AppLogViewModel>()
            {
                new AppLogViewModel(),
                new AppLogViewModel()
            };
            var mockReader = new Mock<IAppLogsReader>();
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<long>())).Returns(appLogs);

            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            var submissionServiceMock = new Mock<ISubmissionService>();
            submissionServiceMock.Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<long>())).Returns(Task.FromResult(new IlrJob()));

            var controller = new AppLogsController(mockReader.Object, submissionServiceMock.Object, mockLogger.Object);
            var result = controller.Index(It.IsAny<long>());

            result.Should().BeOfType(typeof(Task<ViewResult>));
            var modelresult = ((ViewResult)result.Result).Model;
            Assert.IsAssignableFrom<IEnumerable<AppLogViewModel>>(modelresult);
            ((IEnumerable<AppLogViewModel>)modelresult).Count().Should().Be(2);
        }
    }
}