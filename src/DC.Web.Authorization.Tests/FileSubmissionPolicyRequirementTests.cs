using System;
using System.Collections.Generic;
using System.Text;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Query;
using DC.Web.Authorization.Requirements;
using FluentAssertions;
using Moq;
using Xunit;

namespace DC.Web.Authorization.Tests
{
    public class FileSubmissionPolicyRequirementTests
    {
        [Fact]
        public void FileSubmissionPolicyRequirement_True()
        {
            var policyRequirement = new FileSubmissionPolicyRequirement();
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);
            queryServiceMock.Setup(x => x.HasPermission("DAA", new List<string> { FeatureNames.FileSubmission }))
                .Returns(true);
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim { Type = IdamsClaimTypes.Service, Value = "DAA" }
            };
            var result = policyService.IsRequirementMet(claims, policyRequirement);
            result.Should().BeTrue();
            queryServiceMock.Verify(x => x.HasPermission("DAA", new List<string> { FeatureNames.FileSubmission }), Times.Once);
        }

        [Fact]
        public void FileSubmissionPolicyRequirement_False()
        {
            var policyRequirement = new FileSubmissionPolicyRequirement();
            var queryServiceMock = new Mock<IPermissionsQueryService>();
            var policyService = new PolicyService(queryServiceMock.Object);
            queryServiceMock.Setup(x => x.HasPermission("DAA", new List<string> { FeatureNames.ReportViewing }))
                .Returns(true);
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim { Type = IdamsClaimTypes.Service, Value = "DAA" }
            };
            var result = policyService.IsRequirementMet(claims, policyRequirement);
            result.Should().BeFalse();
            queryServiceMock.Verify(x => x.HasPermission("DAA", new List<string> { FeatureNames.FileSubmission }), Times.Once);
        }
    }
}
