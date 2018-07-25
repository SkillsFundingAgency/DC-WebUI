using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Base;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Tests.HelperClasses
{
    public class AuthorizationAuthorizationPolicyHandlerMock : AuthorizationAuthorizationPolicyHandler
    {
        public AuthorizationAuthorizationPolicyHandlerMock(IAuthorizationPolicyService authorizationPolicyService, AuthenticationSettings authenticationSettings)
            : base(authorizationPolicyService, authenticationSettings)
        {
        }

        public Task HandleAsyncTest(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
        {
            return HandleAsync(context, requirement);
        }
    }
}
