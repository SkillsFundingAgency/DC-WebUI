using System;
using System.Collections.Generic;
using DC.Web.Authorization.Data;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.Data.Entities;
using DC.Web.Authorization.Data.Repository;
using DC.Web.Authorization.Query;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DC.Web.Authorization.Tests
{
    public class PermissionsQueryServiceTests
    {
        [Fact]
        public void HasPermissions_True()
        {
            var mockRepository = new Mock<IAuthorizeRepository>();
            var data = new List<RoleFeature>
            {
                new RoleFeature()
                {
                    FeatureId = 1,
                    Feature = new Feature { Id = 1, Name = FeatureNames.FileSubmission },
                    RoleId = 1,
                    Role = new Role { Id = 1, Name = "DAA" }
                }
            };
            mockRepository.Setup(m => m.GetAllRoleFeatures()).Returns(data);
            var permissionsService = new PermissionsQueryService(mockRepository.Object);
            var result = permissionsService.HasPermission("DAA", new List<string>() { FeatureNames.FileSubmission });
            result.Should().BeTrue();
        }

        [Fact]
        public void HasPermissions_False_FeatureNotMatched()
        {
            var mockRepository = new Mock<IAuthorizeRepository>();
            var data = new List<RoleFeature>
            {
                new RoleFeature()
                {
                    FeatureId = 1,
                    Feature = new Feature { Id = 1, Name = FeatureNames.ReportViewing },
                    RoleId = 1,
                    Role = new Role { Id = 1, Name = "DAA" }
                }
            };
            mockRepository.Setup(m => m.GetAllRoleFeatures()).Returns(data);
            var permissionsService = new PermissionsQueryService(mockRepository.Object);
            var result = permissionsService.HasPermission("DAA", new List<string>() { FeatureNames.FileSubmission });
            result.Should().BeFalse();
        }

        [Fact]
        public void HasPermissions_False_RoleNotMatched()
        {
            var mockRepository = new Mock<IAuthorizeRepository>();
            var data = new List<RoleFeature>
            {
                new RoleFeature()
                {
                    FeatureId = 1,
                    Feature = new Feature { Id = 1, Name = FeatureNames.FileSubmission },
                    RoleId = 1,
                    Role = new Role { Id = 1, Name = "DAAXXXX" }
                }
            };
            mockRepository.Setup(m => m.GetAllRoleFeatures()).Returns(data);
            var permissionsService = new PermissionsQueryService(mockRepository.Object);
            var result = permissionsService.HasPermission("DAA", new List<string>() { FeatureNames.FileSubmission });
            result.Should().BeFalse();
        }
    }
}
