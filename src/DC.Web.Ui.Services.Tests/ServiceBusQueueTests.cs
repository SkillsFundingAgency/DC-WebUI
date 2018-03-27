using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.ServiceBus;
using DC.Web.Ui.Settings.Models;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class ServiceBusQueueTests
    {
        [Fact]
        public async Task SendMessage_Success()
        {
            var queueClient = new Mock<IQueueClient>();

            var service = new ServiceBusQueue(queueClient.Object);

            var task = service.SendMessagesAsync("Test", "testid");
            await task.ConfigureAwait(false);
            task.IsCompletedSuccessfully.Should().BeTrue();
            queueClient.Verify(e => e.SendAsync(It.IsAny<Message>()), Times.Exactly(1));
            queueClient.Verify(e => e.CloseAsync(), Times.Exactly(1));

        }

        [Fact]
        public async Task SendMessage_Failure()
        {
            var queueClient = new Mock<IQueueClient>();

            var service = new ServiceBusQueue(queueClient.Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.SendMessagesAsync(null, "testid"));

            queueClient.Verify(e => e.SendAsync(It.IsAny<Message>()), Times.Never);
            queueClient.Verify(e => e.CloseAsync(), Times.Never);

        }

    }
}
