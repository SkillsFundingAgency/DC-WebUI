﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.JobQueue;
using DC.Web.Ui.Settings.Models;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Moq;
using Polly;
using Polly.NoOp;
using Polly.Registry;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class JobQueueServiceTests
    {
        [Fact]
        public async Task AddJobAsync_Success()
        {
            var job = new Job();
            var httpClientMock = new Mock<IBespokeHttpClient>();
            httpClientMock.Setup(x => x.SendDataAsync("test", job));

            var pollyRegistryMock = new Mock<IReadOnlyPolicyRegistry<string>>();
            pollyRegistryMock.Setup(x => x.Get<IAsyncPolicy>("HttpRetryPolicy")).Returns(Policy.NoOpAsync);
            var apiSettings = new JobQueueApiSettings()
            {
                BaseUrl = string.Empty
            };
            var service = new JobQueueService(apiSettings, pollyRegistryMock.Object, httpClientMock.Object);

            var task = service.AddJobAsync(job);
            await task.ConfigureAwait(false);
            task.IsCompletedSuccessfully.Should().BeTrue();
            httpClientMock.Verify(e => e.SendDataAsync(It.IsAny<string>(), It.IsAny<Job>()), Times.Exactly(1));
        }
    }
}