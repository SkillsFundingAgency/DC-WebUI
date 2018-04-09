using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Data.Entities;
using DC.Web.Authorization.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DC.Web.Authorization.Data.Tests
{
    public class AuthorizeRepositoryTests
    {
        [Fact]
        public void GetAllRoleFeatures_Test_Success()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("Test_Database_RoleFeature")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                var feature = new Feature { Id = 1, Name = "test feature", Description = "test desc" };
                context.Features.Add(feature);

                var role = new Role { Id = 1, Name = "test role", Description = "test desc" };
                context.Roles.Add(role);

                context.RoleFeatures.Add(new RoleFeature { Id = 1, FeatureId = 1, Role = role, Feature = feature });
                context.SaveChanges();
            }

            var repository = new AuthorizeRepository(new AuthorizeDbContext(options));

            var roleFeatures = repository.GetAllRoleFeatures();
            roleFeatures.Count().Should().Be(1);

            var roleFeature = roleFeatures.FirstOrDefault();

            roleFeature.Should().NotBeNull();
            roleFeature.RoleId.Should().Be(1);
            roleFeature.FeatureId.Should().Be(1);
            roleFeature.Feature.Should().NotBeNull();
            roleFeature.Role.Should().NotBeNull();
        }

        [Fact]
        public void GetAllRoleFeatures_Test_Null()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("Test_Database_RoleFeature")
                .Options;

            var context = new AuthorizeDbContext(options);
            var repository = new AuthorizeRepository(context);

            var roleFeatures = repository.GetAllRoleFeatures();

            roleFeatures.Count().Should().Be(0);
            context.RoleFeatures.FirstOrDefault().Should().BeNull();
        }
    }
}