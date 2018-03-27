using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.ClaimTypes;
using DC.Web.Ui.Services.ClaimHandlerService;
using DC.Web.Ui.Services.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Ui.Security
{
    public class ValidUkprnAuthoriseHandler : AuthorizationHandler<UkprnRequirement>
    {
        private IUkprnClaimHandlerService _ukprnClaimsHandler;
        public ValidUkprnAuthoriseHandler(IUkprnClaimHandlerService ukprnClaimsHandler)
        {
            _ukprnClaimsHandler = ukprnClaimsHandler;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,UkprnRequirement requirement)
        {

            //context.Succeed(requirement);

            var idamsClaims = context.User.Claims.Select(x => new IdamsClaim()
            {
                Type = x.Type,
                Value = x.Value
            });

            if (_ukprnClaimsHandler.ClaimAccepted(idamsClaims))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;

        }
    }

    public class UkprnRequirement : IAuthorizationRequirement
    {

    }
}
