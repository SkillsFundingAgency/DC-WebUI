//using System;
//using DC.Web.Authorization.Tests.HelperClasses;
//using DC.Web.Ui.Settings.Models;
//using FluentAssertions;
//using Microsoft.AspNetCore.Authorization;
//using Moq;
//using Xunit;

//namespace DC.Web.Authorization.Tests
//{
//    public class PolicyHandlerBaseTests
//    {
//        [Fact]
//        public void AutenticationEnabled_False()
//        {
//            var authSettings = new AuthenticationSettings { Enabled = false };
//            var policyhandlerBaseMock = new AuthorizationPolicyBaseHandlerMock(authSettings);
//            var requirementMock = new Mock<IAuthorizationRequirement>();
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, null, null);

//            var result = policyhandlerBaseMock.HandleRequirementAsyncTest(
//               authorizationHandlerContext,
//                requirementMock.Object);
//            authorizationHandlerContext.HasSucceeded.Should().BeTrue();
//            result.IsCompleted.Should().BeTrue();
//        }

//        [Fact]
//        public void AutenticationEnabled_True()
//        {
//            var authSettings = new AuthenticationSettings { Enabled = true };
//            var policyhandlerBaseMock = new AuthorizationPolicyBaseHandlerMock(authSettings);
//            var requirementMock = new Mock<IAuthorizationRequirement>();
//            var authorizationHandlerContext = new AuthorizationHandlerContext(new[] { requirementMock.Object }, null, null);

//           Assert.Throws<NotImplementedException>(() => policyhandlerBaseMock.HandleRequirementAsyncTest(
//                authorizationHandlerContext,
//                requirementMock.Object).Should());
//        }
//    }
//}