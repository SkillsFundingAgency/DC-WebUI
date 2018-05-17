using System.Linq;
using DC.Web.Authorization.Data.SeedData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DC.Web.Authorization.Data.Tests
{
    public class AuthorizationDataSeederTests
    {
        [Fact]
        public void AuthorizationDataSeeder_InitialiseTest()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("AuthorizationDataSeeder_InitialiseTest")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                AuthorizationDataSeeder.Initialize(context);
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.Roles.Count().Should().Be(4);
                context.Features.Count().Should().Be(2);
                context.RoleFeatures.Count().Should().Be(2);
            }
        }

        [Fact]
        public void AddRoles_Test()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("AddRoles_Test")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                AuthorizationDataSeeder.AddRoles(context);
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.Roles.Count().Should().Be(4);
            }
        }

        [Fact]
        public void AddFeatures_Test()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("AddFeatures_Test")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                AuthorizationDataSeeder.AddFeatures(context);
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.Features.Count().Should().Be(2);
            }
        }

        [Fact]
        public void AddRoleFeatures_Test()
        {
            var options = new DbContextOptionsBuilder<AuthorizeDbContext>()
                .UseInMemoryDatabase("AddRoleFeatures_Test")
                .Options;

            using (var context = new AuthorizeDbContext(options))
            {
                AuthorizationDataSeeder.AddRoleFeatures(context);
            }

            using (var context = new AuthorizeDbContext(options))
            {
                context.RoleFeatures.Count().Should().Be(2);
            }
        }
    }
}