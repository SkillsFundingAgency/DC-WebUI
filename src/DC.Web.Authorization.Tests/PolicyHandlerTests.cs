//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using DC.Web.Authorization.Base;
//using DC.Web.Authorization.Idams;
//using DC.Web.Authorization.PlicyRequirements;
//using DC.Web.Authorization.Tests.HelperClasses;
//using DC.Web.Ui.Settings.Models;
//using FluentAssertions;
//using Microsoft.AspNetCore.Authorization;
//using Moq;
//using Xunit;

//namespace DC.Web.Authorization.Tests
//{
//    public class PolicyHandlerTests
//    {
//        [Fact]
//        public void NullClaims_Fail()
//        {
//            var requirementMock = new Mock<FileSubmissionPolicyRequirement>();
//            var policyServiceMock = new Mock<IAuthorizationPolicyService>();
//            var policyhandlerBaseMock = new AuthorizationAuthorizationPolicyHandlerMock(policyServiceMock.Object, It.IsAny<AuthenticationSettings>());
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, null, null);

//            var result = policyhandlerBaseMock.HandleAsyncTest(
//               authorizationHandlerContext,
//                requirementMock.Object);

//            policyServiceMock.Verify(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object), Times.Never);
//            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
//            authorizationHandlerContext.HasFailed.Should().BeTrue();
//            result.IsCompleted.Should().BeTrue();
//        }

//        [Fact]
//        public void EmptyClaims_Fail()
//        {
//            var requirementMock = new Mock<FileSubmissionPolicyRequirement>();
//            var policyServiceMock = new Mock<IAuthorizationPolicyService>();
//            var policyhandlerBaseMock =
//                new AuthorizationAuthorizationPolicyHandlerMock(policyServiceMock.Object, It.IsAny<AuthenticationSettings>());
//            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
//            var claimsPrincipal = new ClaimsPrincipal(identity);
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, claimsPrincipal, null);

//            var result = policyhandlerBaseMock.HandleAsyncTest(
//                authorizationHandlerContext,
//                requirementMock.Object);

//            policyServiceMock.Verify(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object), Times.Never);
//            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
//            authorizationHandlerContext.HasFailed.Should().BeTrue();
//            result.IsCompleted.Should().BeTrue();
//        }

//        [Fact]
//        public void InValidClaims_Fail()
//        {
//            var requirementMock = new Mock<FileSubmissionPolicyRequirement>();
//            var policyServiceMock = new Mock<IAuthorizationPolicyService>();

//            policyServiceMock.Setup(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object)).Returns(false);
//            var policyhandlerBaseMock =
//                new AuthorizationAuthorizationPolicyHandlerMock(policyServiceMock.Object, It.IsAny<AuthenticationSettings>());

//            var claims = new List<Claim>() { new Claim(IdamsClaimTypes.DisplayName, "test") };
//            var identity = new ClaimsIdentity(claims, "Idams");
//            var claimsPrincipal = new ClaimsPrincipal(identity);
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, claimsPrincipal, null);

//            var result = policyhandlerBaseMock.HandleAsyncTest(
//                authorizationHandlerContext,
//                requirementMock.Object);

//            policyServiceMock.Verify(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object), Times.Once);
//            authorizationHandlerContext.HasSucceeded.Should().BeFalse();
//            authorizationHandlerContext.HasFailed.Should().BeTrue();
//            result.IsCompleted.Should().BeTrue();
//        }

//        [Theory]
//        [InlineData("DAA")]
//        [InlineData("DCS")]
//        public void ValidClaims_Success(string role)
//        {
//            var requirementMock = new Mock<FileSubmissionPolicyRequirement>();
//            var policyServiceMock = new Mock<IAuthorizationPolicyService>();

//            policyServiceMock.Setup(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object)).Returns(true);
//            var policyhandlerBaseMock =
//                new AuthorizationAuthorizationPolicyHandlerMock(policyServiceMock.Object, It.IsAny<AuthenticationSettings>());

//            var claims = new List<Claim>() { new Claim(IdamsClaimTypes.DisplayName, role) };
//            var identity = new ClaimsIdentity(claims, "Idams");
//            var claimsPrincipal = new ClaimsPrincipal(identity);
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, claimsPrincipal, null);

//            var result = policyhandlerBaseMock.HandleAsyncTest(
//                authorizationHandlerContext,
//                requirementMock.Object);

//            policyServiceMock.Verify(x => x.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), requirementMock.Object), Times.Once);
//            authorizationHandlerContext.HasSucceeded.Should().BeTrue();
//            authorizationHandlerContext.HasFailed.Should().BeFalse();
//            result.IsCompleted.Should().BeTrue();
//        }
//    }
//}