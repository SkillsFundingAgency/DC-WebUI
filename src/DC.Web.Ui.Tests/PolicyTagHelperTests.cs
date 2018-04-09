using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.TagHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests
{
    public class PolicyTagHelperTests
    {
        [Fact]
        public void Suppress_Output_True()
        {
            var tagHelperOutput = SetupTagHelper(false);
            tagHelperOutput.TagName.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Suppress_Output_False()
        {
            var tagHelperOutput = SetupTagHelper(true);
            tagHelperOutput.TagName.Should().NotBeEmpty();
        }

        private TagHelperOutput SetupTagHelper(bool authSuccess)
        {
            var authService = new AuthorizationServiceWrapper(authSuccess);
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = claimsPrincipal
            });

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput(
                "policy",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.AppendHtml(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            var tagHelper = new PolicyTagHelper(authService, httpContextAccessorMock.Object)
            {
                Policy = "policy"
            };

            var processResult = tagHelper.ProcessAsync(tagHelperContext, tagHelperOutput);
            processResult.ConfigureAwait(false);
            processResult.IsCompletedSuccessfully.Should().BeTrue();
            return tagHelperOutput;
        }
    }
}
