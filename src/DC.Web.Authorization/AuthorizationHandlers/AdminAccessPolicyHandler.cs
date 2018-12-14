using System;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Idams;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.AuthorizationHandlers
{
    public class AdminAccessPolicyHandler : AuthorizationPolicyHandlerBase<AdminAccessPolicyRequirement>
    {
        public AdminAccessPolicyHandler(AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, AdminAccessPolicyRequirement requirement)
        {
            if (context.User?.Claims == null || !context.User.Claims.Any())
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var idamsClaims = context.User.Claims.Select(x => new IdamsClaim()
            {
                Type = x.Type,
                Value = x.Value
            });

            if (idamsClaims.Any(x => x.Type == IdamsClaimTypes.UserType && x.Value.Equals("Admin", StringComparison.InvariantCultureIgnoreCase)))
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
}