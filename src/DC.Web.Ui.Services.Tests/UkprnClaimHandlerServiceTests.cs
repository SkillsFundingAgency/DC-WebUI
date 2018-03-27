using System.Collections.Generic;
using DC.Web.Ui.Services.ClaimHandlerService;
using DC.Web.Ui.Services.Models;
using FluentAssertions;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class UkprnClaimHandlerServiceTests
    {
        [Fact]
        public void TestClaimAccepted_True()
        {
            var service = new UkprnClaimHandlerService();
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim() {Type = "Ukprn", Value = "1"},
                new IdamsClaim() {Type = "UkprnX", Value = "10"},
            };
            service.ClaimAccepted(claims).Should().BeTrue();
        }

        [Fact]
        public void TestClaimAccepted_False()
        {
            var service = new UkprnClaimHandlerService();
            var claims = new List<IdamsClaim>()
            {
                new IdamsClaim() {Type = "UkprnYY", Value = "1"},
                new IdamsClaim() {Type = "UkprnXXX", Value = "10"},
            };
            service.ClaimAccepted(claims).Should().BeFalse();
        }


        [Fact]
        public void TestClaimAccepted_Null()
        {
            var service = new UkprnClaimHandlerService();
            service.ClaimAccepted(null).Should().BeFalse();
        }
    }
}
