using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class AppLogsReaderTests
    {
        [Fact]
        public void GetApplicationLogs_ReturnsItems_Test()
        {
            var logs = new List<AppLog>()
            {
                new AppLog(){JobId = "TestJob", Message = "test message"},
                new AppLog(){JobId = "TestJob2", Message = "test message"},
                new AppLog(){JobId = "TestJob", Message = "test message3"},
                new AppLog(){JobId = "TestJob", Message = "test message4"},
            }.AsQueryable();

            var mockContext = SetupMockContext(logs);

            var service = new AppLogsReader(mockContext.Object);
            service.GetApplicationLogs("TestJob").Count().Should().Be(3);
        }

        [Fact]
        public void GetApplicationLogs_NoItemsReturned_Test()
        {
            var logs = new List<AppLog>()
            {
                new AppLog(){JobId = "TestJob2", Message = "test message"},
            }.AsQueryable();

            var mockContext = SetupMockContext(logs);

            var service = new AppLogsReader(mockContext.Object);
            service.GetApplicationLogs("TestJob").Count().Should().Be(0);
        }

        private Mock<AppLogsContext> SetupMockContext(IQueryable<AppLog> logs)
        {
            var mockSet = new Mock<DbSet<AppLog>>();
            mockSet.As<IQueryable<AppLog>>().Setup(m => m.Provider).Returns(logs.Provider);
            mockSet.As<IQueryable<AppLog>>().Setup(m => m.Expression).Returns(logs.Expression);
            mockSet.As<IQueryable<AppLog>>().Setup(m => m.ElementType).Returns(logs.ElementType);
            mockSet.As<IQueryable<AppLog>>().Setup(m => m.GetEnumerator()).Returns(() => logs.GetEnumerator());

            var mockContext = new Mock<AppLogsContext>();
            mockContext.Setup(c => c.Logs).Returns(mockSet.Object);
            return mockContext;
        }
    }
}
