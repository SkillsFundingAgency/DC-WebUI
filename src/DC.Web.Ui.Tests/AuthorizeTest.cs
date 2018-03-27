using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace DC.Web.Ui.Tests
{
    public class AuthorizeTest
    {
        [Theory]
        [InlineData("AppLogs/Index")]
        [InlineData("IlrSubmission/Index")]
        [InlineData("Confirmation/Index")]
        [InlineData("Claims/Index")]
        public async Task GetAsync_ReturnsUnauthorizedResult(string url)
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            
            var client = server.CreateClient();
            
            var response = await client.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.Found);
            response.Headers.Location.AbsoluteUri.Should().Contain("https://adfs.");
            
        }
    }

    
}
