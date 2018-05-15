using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Requirements;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Tests.HelperClasses
{
    public class PolicyHandlerMock : PolicyHandler
    {
        public PolicyHandlerMock(IPolicyService policyService, AuthenticationSettings authenticationSettings)
            : base(policyService, authenticationSettings)
        {
        }

        public Task HandleAsyncTest(AuthorizationHandlerContext context, IPolicyRequirement requirement)
        {
            return HandleAsync(context, requirement);
        }
    }
}
