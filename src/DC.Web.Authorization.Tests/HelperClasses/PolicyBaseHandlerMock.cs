using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Requirements;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Tests.HelperClasses
{
    public class PolicyBaseHandlerMock : PolicyHandlerBase<IPolicyRequirement>
    {
        public PolicyBaseHandlerMock(AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
        }

        public Task HandleRequirementAsyncTest(AuthorizationHandlerContext context, IPolicyRequirement requirement)
        {
            return HandleRequirementAsync(context, requirement);
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, IPolicyRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}
