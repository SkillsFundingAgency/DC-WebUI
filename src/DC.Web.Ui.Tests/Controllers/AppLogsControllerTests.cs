using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.JobQueueManager.Models;
using ESFA.DC.Logging.Interfaces;
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
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<long>())).Returns(It.IsAny<IEnumerable<AppLog>>());

            var controller = new AppLogsController(mockReader.Object, null, mockLogger.Object, new AuthenticationSettings());
            var result = controller.Index(It.IsAny<long>());

            result.Should().BeOfType(typeof(Task<ViewResult>));
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
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<long>())).Returns(appLogs);

            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            var submissionServiceMock = new Mock<ISubmissionService>();
            submissionServiceMock.Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<long>())).Returns(Task.FromResult(new Job()));

            var controller = new AppLogsController(mockReader.Object, submissionServiceMock.Object, mockLogger.Object, new AuthenticationSettings());
            var result = controller.Index(It.IsAny<long>());

            result.Should().BeOfType(typeof(Task<ViewResult>));
            var modelresult = ((ViewResult)result.Result).Model;
            Assert.IsAssignableFrom<IEnumerable<AppLog>>(modelresult);
            ((IEnumerable<AppLog>)modelresult).Count().Should().Be(2);
        }
    }
}