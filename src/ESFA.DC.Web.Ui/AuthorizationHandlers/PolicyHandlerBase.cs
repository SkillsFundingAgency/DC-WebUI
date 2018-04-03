using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Ui.AuthorizationHandlers
{
    public abstract class PolicyHandlerBase<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
    {
        private readonly AuthenticationSettings _authenticationSettings;
        protected PolicyHandlerBase(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        protected sealed override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
        {
            if (!_authenticationSettings.Enabled)
            {
                context.Succeed(requirement);
                return Task.FromResult(true);
            }
            else
            {
                return HandleAsync(context, requirement);
            }
        }

        protected abstract Task HandleAsync(AuthorizationHandlerContext context, T requirement);
    }
}
