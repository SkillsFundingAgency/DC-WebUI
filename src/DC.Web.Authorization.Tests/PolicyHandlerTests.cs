using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Policies;
using DC.Web.Ui.Settings.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace DC.Web.Authorization.Tests
{
    public class PolicyHandlerTests
    {
        [Fact]
        public void NullClaims_Fail()
        {
            var policyhandler = new AuthorizationPolicyHandler();
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new FileSubmissionPolicyRequirement(),
                    new HelpDeskAccessPolicyRequirement(),
                },
                null,
                null);

            var result = policyhandler.HandleAsync(authorizationHandlerContext);

            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
            authorizationHandlerContext.HasFailed.Should().BeTrue();
            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void EmptyClaims_Fail()
        {
            var policyhandler = new AuthorizationPolicyHandler();
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new FileSubmissionPolicyRequirement(),
                    new HelpDeskAccessPolicyRequirement(),
                },
                claimsPrincipal,
                null);

            var result = policyhandler.HandleAsync(authorizationHandlerContext);

            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
            authorizationHandlerContext.HasFailed.Should().BeTrue();
            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void InValidClaims_Fail()
        {
            var policyhandler = new AuthorizationPolicyHandler();

            var claims = new List<Claim>()
            {
                new Claim(IdamsClaimTypes.DisplayName, "test"),
                new Claim(IdamsClaimTypes.Service, "XYZ"),
                new Claim(IdamsClaimTypes.UserType, "ABC")
            };
            var identity = new ClaimsIdentity(claims, "Idams");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new FileSubmissionPolicyRequirement(),
                },
                claimsPrincipal,
                null);

            var result = policyhandler.HandleAsync(authorizationHandlerContext);
            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
            result.IsCompleted.Should().BeTrue();
        }

        [Theory]
        [InlineData("DAA")]
        [InlineData("DCS")]
        public void ValidClaims_Success_FileSubmission(string role)
        {
            var policyhandler = new AuthorizationPolicyHandler();

            var claims = new List<Claim>()
            {
                new Claim(IdamsClaimTypes.DisplayName, "test"),
                new Claim(IdamsClaimTypes.Service, role),
            };
            var identity = new ClaimsIdentity(claims, "Idams");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new FileSubmissionPolicyRequirement(),
                },
                claimsPrincipal,
                null);

            var result = policyhandler.HandleAsync(authorizationHandlerContext);
            authorizationHandlerContext.HasSucceeded.Should().BeTrue();
            authorizationHandlerContext.HasFailed.Should().BeFalse();
            result.IsCompleted.Should().BeTrue();
        }

        [Theory]
        [InlineData("DAA")]
        [InlineData("DCS")]
        public void ValidClaims_Fail_AdminArea(string role)
        {
            var policyhandler = new AuthorizationPolicyHandler();

            var claims = new List<Claim>()
            {
                new Claim(IdamsClaimTypes.DisplayName, "test"),
                new Claim(IdamsClaimTypes.Service, role),
            };
            var identity = new ClaimsIdentity(claims, "Idams");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new HelpDeskAccessPolicyRequirement(),
                },
                claimsPrincipal,
                null);

            var result = policyhandler.HandleAsync(authorizationHandlerContext);
            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task ValidClaims_Fail_FileSubmission()
        {
            var policyhandler = new AuthorizationPolicyHandler();

            var claims = new List<Claim>()
            {
                new Claim(IdamsClaimTypes.DisplayName, "test"),
                new Claim(IdamsClaimTypes.UserType, "LSC"),
            };
            var identity = new ClaimsIdentity(claims, "Idams");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new FileSubmissionPolicyRequirement(),
                },
                claimsPrincipal,
                new object());

            await policyhandler.HandleAsync(authorizationHandlerContext);
            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public async Task ValidClaims_Success_Admin()
        {
            var policyhandler = new AuthorizationPolicyHandler();

            var claims = new List<Claim>()
            {
                new Claim(IdamsClaimTypes.DisplayName, "test"),
                new Claim(IdamsClaimTypes.UserType, "LSC"),
            };
            var identity = new ClaimsIdentity(claims, "Idams");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>
                {
                    new HelpDeskAccessPolicyRequirement(),
                },
                claimsPrincipal,
                new object());

            await policyhandler.HandleAsync(authorizationHandlerContext);
            authorizationHandlerContext.HasSucceeded.Should().BeTrue();
            authorizationHandlerContext.HasFailed.Should().BeFalse();
        }
    }
}