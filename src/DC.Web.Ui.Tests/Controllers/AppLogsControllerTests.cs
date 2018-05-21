using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.SubmissionService;
using ESFA.DC.JobQueueManager.Models;
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
            var mockReader = new Mock<IAppLogsReader>();
            mockReader.Setup(x => x.GetApplicationLogs(It.IsAny<long>())).Returns(It.IsAny<IEnumerable<AppLog>>());

            var controller = new AppLogsController(mockReader.Object, null);
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

            var submissionServiceMock = new Mock<ISubmissionService>();
            submissionServiceMock.Setup(x => x.GetJob(It.IsAny<long>())).Returns(Task.FromResult(new Job()));

            var controller = new AppLogsController(mockReader.Object, submissionServiceMock.Object);
            var result = controller.Index(It.IsAny<long>());

            result.Should().BeOfType(typeof(Task<ViewResult>));
            var modelresult = ((ViewResult)result.Result).Model;
            Assert.IsAssignableFrom<IEnumerable<AppLog>>(modelresult);
            ((IEnumerable<AppLog>)modelresult).Count().Should().Be(2);
        }
    }
}