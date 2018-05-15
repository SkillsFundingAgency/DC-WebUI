using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DC.Web.Authorization.Data.Entities;
using DC.Web.Authorization.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DC.Web.Authorization.Data.Tests
{
    public class AuthorizeDbContextTests
    {
        [Fact]
        public void RoleTest()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("Test_Database_Role")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                context.Roles.Add(new Role { Id = 1, Name = "test role", Description = "test desc" });
                context.SaveChanges();
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.Roles.Count().Should().Be(1);
                var role = context.Roles.First();

                role.Id.Should().Be(1);
                role.Description.Should().Be("test desc");
                role.Name.Should().Be("test role");
            }
        }

        [Fact]
        public void FeatureTest()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("Test_Database_Feature")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                context.Features.Add(new Feature { Id = 1, Name = "test feature", Description = "test desc" });
                context.SaveChanges();
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.Features.Count().Should().Be(1);
                var feature = context.Features.First();

                feature.Id.Should().Be(1);
                feature.Description.Should().Be("test desc");
                feature.Name.Should().Be("test feature");
            }
        }

        [Fact]
        public void RoleFeatureTest()
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

            using (var context = new AuthorizeDbContext(options))
            {
                context.RoleFeatures.Count().Should().Be(1);
                var roleFeature = context.RoleFeatures
                    .Include(x => x.Role)
                    .Include(x => x.Feature)
                    .First();

                roleFeature.RoleId.Should().Be(1);
                roleFeature.FeatureId.Should().Be(1);
                roleFeature.Feature.Should().NotBeNull();
                roleFeature.Role.Should().NotBeNull();
            }
        }
    }
}
