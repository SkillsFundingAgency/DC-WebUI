using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Query;
using DC.Web.Authorization.Requirements;
using FluentAssertions;
using Moq;
using Xunit;

namespace DC.Web.Authorization.Tests
{
    public class PolicyServiceTests
    {
        [Fact]
        public void IsRequirementmet_False_NullClaims()
        {
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);
            var result = policyService.IsRequirementMet(null, new Mock<IPolicyRequirement>().Object);

            result.Should().BeFalse();
            queryServiceMock.Verify(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Fact]
        public void IsRequirementmet_False_NullRequirement()
        {
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);

            var result = policyService.IsRequirementMet(It.IsAny<IEnumerable<IdamsClaim>>(), null);
            result.Should().BeFalse();
            queryServiceMock.Verify(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Fact]
        public void IsRequirementmet_False_No_ServieClaims()
        {
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);
            queryServiceMock.Setup(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(false);
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim { Type = IdamsClaimTypes.DisplayName, Value = string.Empty }
            };
            var result = policyService.IsRequirementMet(claims, new Mock<IPolicyRequirement>().Object);
            result.Should().BeFalse();
            queryServiceMock.Verify(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Fact]
        public void IsRequirementmet_True()
        {
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);
            queryServiceMock.Setup(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true);
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim { Type = IdamsClaimTypes.Service, Value = string.Empty }
            };
            var result = policyService.IsRequirementMet(claims, new Mock<IPolicyRequirement>().Object);
            result.Should().BeTrue();
            queryServiceMock.Verify(x => x.HasPermission(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
