using DC.Web.Ui.Controllers;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.IO;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace DC.Web.Ui.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public void Index_Test_SignIn()
        {

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("callbackUrl")
                .Verifiable();

            var controller = new AccountController
            {
                Url = mockUrlHelper.Object
            };
            controller.SignIn().Should().BeOfType(typeof(ChallengeResult));
        }

        [Fact]
        public void Index_Test_PostSignIn()
        {
            var controller = new AccountController();
            controller.PostSignIn().Should().BeOfType(typeof(RedirectToActionResult));
        }

      
    }
}
