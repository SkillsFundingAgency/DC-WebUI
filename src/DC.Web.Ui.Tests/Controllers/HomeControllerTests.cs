﻿using System.Security.Claims;
using System.Security.Principal;
using DC.Web.Ui.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeControllerTests_Index_Test_NotAutnenticated()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Object.HttpContext = new DefaultHttpContext();
            mockContext.Object.HttpContext.User = new ClaimsPrincipal();

            var controller = new HomeController
            {
                ControllerContext = mockContext.Object
            };

            var result = controller.Index();
            result.Should().BeOfType(typeof(ViewResult));
        }

        [Fact]
        public void HomeControllerTests_Index_Test_Authenticated()
        {
            var context = new ControllerContext();
            context.HttpContext = new DefaultHttpContext
            {
                User = new GenericPrincipal(
                    new GenericIdentity("username"),
                    new string[0])
            };
            var controller = new HomeController
            {
                ControllerContext = context
            };
            controller.Index().Should().BeOfType(typeof(RedirectToActionResult));
        }
    }
}